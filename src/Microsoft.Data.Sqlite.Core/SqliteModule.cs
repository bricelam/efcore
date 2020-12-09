#pragma warning disable CS1591, SA1600

namespace Microsoft.Data.Sqlite
{
    public class SqliteVirtualTable
    {
        public SqliteConnection Connection { get; }
        public string CreateTableSql { get; }
        public SqliteModule Module { get; }
    }

    // TODO: How to declare as eponymous (Create and Connect are the same) or eponymous-only (no Create)?
    public abstract class SqliteModule
    {
        // (optional)
        public abstract SqliteVirtualTable Create(SqliteConnection connection, object state, string module, string database, string table, string[] args);

        public abstract SqliteVirtualTable Connect(SqliteConnection connection, object state, string module, string database, string table, string[] args);

        //    int (* xBestIndex) (sqlite3_vtab* pVTab, sqlite3_index_info*);
        //    int (* xDisconnect) (sqlite3_vtab* pVTab);
        //    (optional) int (* xDestroy) (sqlite3_vtab* pVTab);
        //    int (* xOpen) (sqlite3_vtab* pVTab, sqlite3_vtab_cursor** ppCursor);
        //    int (* xClose) (sqlite3_vtab_cursor*);
        //    int (* xFilter) (sqlite3_vtab_cursor*, int idxNum, const char* idxStr, int argc, sqlite3_value **argv);
        //    int (* xNext) (sqlite3_vtab_cursor*);
        //    int (* xEof) (sqlite3_vtab_cursor*);
        //    int (* xColumn) (sqlite3_vtab_cursor*, sqlite3_context*, int);
        //    int (* xRowid) (sqlite3_vtab_cursor*, sqlite3_int64* pRowid);
        //    (optional) int (* xUpdate) (sqlite3_vtab*, int, sqlite3_value**, sqlite3_int64*);
        //    (optional) int (* xBegin) (sqlite3_vtab* pVTab);
        //    (optional) int (* xSync) (sqlite3_vtab* pVTab);
        //    (optional) int (* xCommit) (sqlite3_vtab* pVTab);
        //    (optional) int (* xRollback) (sqlite3_vtab* pVTab);
        //    (optional) int (* xFindFunction) (sqlite3_vtab* pVtab, int nArg, const char* zName,void (** pxFunc) (sqlite3_context*, int, sqlite3_value**),void** ppArg);
        //    (optional) int (* xRename) (sqlite3_vtab* pVtab, const char* zNew);
        //    (optional) int (* xSavepoint) (sqlite3_vtab* pVTab, int);
        //    (optional) int (* xRelease) (sqlite3_vtab* pVTab, int);
        //    (optional) int (* xRollbackTo) (sqlite3_vtab* pVTab, int);
        //    (optional) int (* xShadowName) (const char*);
    }
}
