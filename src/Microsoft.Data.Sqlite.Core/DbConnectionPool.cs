using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.Data.Sqlite
{
    internal class DbConnectionPool
    {
        private const int SEMAPHORE_HANDLE = 0x0;
        private const int ERROR_HANDLE = 0x1;
        private const int CREATION_HANDLE = 0x2;
        private const int BOGUS_HANDLE = 0x3;

        private const int WAIT_TIMEOUT = 0x102;
        private const int WAIT_ABANDONED = 0x80;
        private const int WAIT_FAILED = -1;

        private static readonly Random _random = new Random(5101977);

        private readonly int _cleanupWait;
        private readonly SqliteConnectionFactory _connectionFactory;
        private readonly DbConnectionPoolGroup _connectionPoolGroup;

        private State _state;
        private int _waitCount;
        private readonly PoolWaitHandles _waitHandles;

        public DbConnectionPool(SqliteConnectionFactory connectionFactory, DbConnectionPoolGroup connectionPoolGroup)
        {
            _state = State.Initializing;

            lock (_random)
            {
                _cleanupWait = _random.Next(12, 24) * 10 * 1000;
            }

            _connectionFactory = connectionFactory;

            _connectionPoolGroup = connectionPoolGroup; _state = State.Running;
        }

        public bool IsRunning
            => _state == State.Running;

        public DbConnectionInternal? GetConnection(DbConnectionClosed owningObject)
        {
            if (_state != State.Running)
            {
                return null;
            }

            var waitForMultipleObjectsTimeout = (uint)CreationTimeout;
            if (waitForMultipleObjectsTimeout == 0)
            {
                waitForMultipleObjectsTimeout = unchecked((uint)Timeout.Infinite);
            }

            DbConnectionInternal? obj = null;

            Interlocked.Increment(ref _waitCount);
            var waitHandleCount = 3u;

            do
            {
                var waitResult = BOGUS_HANDLE;
                var releaseSemaphoreResult = 0;

                var mustRelease = false;
                var waitForMultipleObjectsExHR = 0;
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    _waitHandles.DangerousAddRef(ref mustRelease);

                    RuntimeHelpers.PrepareConstrainedRegions();
                    try
                    {
                    }
                    finally
                    {
                        // TODO: Use WaitHandle
                        waitResult = SafeNativeMethods.WaitForMultipleObjectsEx(
                            waitHandleCount,
                            _waitHandles.DangerousGetHandle(),
                            false,
                            waitForMultipleObjectsTimeout,
                            false);
                        if (waitResult == WAIT_FAILED)
                        {
                            waitForMultipleObjectsExHR = Marshal.GetHRForLastWin32Error();
                        }
                    }

                    switch (waitResult)
                    {
                        case WAIT_TIMEOUT:
                            Interlocked.Decrement(ref _waitCount);

                            return null;

                        case ERROR_HANDLE:
                            Interlocked.Decrement(ref _waitCount);

                            throw TryCloneCachedException();

                        case CREATION_HANDLE:
                            try
                            {
                                obj = UserCreateRequest(owningObject);
                            }
                            catch
                            {
                                // Is this possible?
                                if (obj == null)
                                {
                                    Interlocked.Decrement(ref _waitCount);
                                }

                                throw;
                            }
                            finally
                            {
                                if (obj != null)
                                {
                                    Interlocked.Decrement(ref _waitCount);
                                }
                            }

                            if (obj == null)
                            {
                                if (Count >= MaxPoolSize && MaxPoolSize != 0)
                                {
                                    if (!ReclaimEmancipatedObjects())
                                    {
                                        waitHandleCount = 2u;
                                    }
                                }
                            }
                            break;

                        case SEMAPHORE_HANDLE:
                            Interlocked.Decrement(ref _waitCount);
                            obj = GetFromGeneralPool();

                            if (obj != null && !obj.IsConnectionAlive())
                            {
                                DestroyObject(obj);
                                obj = null;

                                // TODO: Why ever unsigned?
                                if (_waitHandles.CreationSemaphore.WaitOne(unchecked((int)waitForMultipleObjectsTimeout)))
                                {
                                    RuntimeHelpers.PrepareConstrainedRegions();
                                    try
                                    {
                                        obj = UserCreateRequest(owningObject);
                                    }
                                    finally
                                    {
                                        _waitHandles.CreationSemaphore.Release(1);
                                    }
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            break;

                        case WAIT_FAILED:
                            Interlocked.Decrement(ref _waitCount);
                            Marshal.ThrowExceptionForHR(waitForMultipleObjectsExHR);
                            goto default;

                        case WAIT_ABANDONED + SEMAPHORE_HANDLE:
                            Interlocked.Decrement(ref _waitCount);
                            throw new AbandonedMutexException(SEMAPHORE_HANDLE, _waitHandles.PoolSemaphore);

                        case WAIT_ABANDONED + ERROR_HANDLE:
                            Interlocked.Decrement(ref _waitCount);
                            throw new AbandonedMutexException(ERROR_HANDLE, _waitHandles.ErrorEvent);

                        case WAIT_ABANDONED + CREATION_HANDLE:
                            Interlocked.Decrement(ref _waitCount);
                            throw new AbandonedMutexException(CREATION_HANDLE, _waitHandles.CreationSemaphore);

                        default:
                            Interlocked.Decrement(ref _waitCount);
                            throw new InvalidOperationException("ADP.InternalErrorCode.UnexpectedWaitAnyResult");
                    }
                }
                finally
                {
                    if (waitResult == CREATION_HANDLE)
                    {
                        // TODO: Use WaitHandle
                        var result = SafeNativeMethods.ReleaseSemaphor(_waitHandles.CreationHandle.DangerousGetHandle(), 1, IntPtr.Zero);
                        if (result == 0)
                        {
                            releaseSemaphoreResult = Marshal.GetHRForLastWin32Error();
                        }
                    }

                    if (mustRelease)
                    {
                        _waitHandles.DangerousRelease();
                    }
                }

                if (releaseSemaphoreResult != 0)
                {
                    Marshal.ThrowExceptionForHR(releaseSemaphoreResult);
                }
            }
            while (obj == null);

            if (obj != null)
            {
                PrepareConnection(owningObject, obj);
            }

            return obj;
        }

        private enum State
        {
            Initializing,
            Running
        }
    }
}
