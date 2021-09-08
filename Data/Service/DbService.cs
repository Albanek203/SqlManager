using System.Data;
using SqlManager.Data.Repository;

namespace SqlManager.Data.Service {
    public class DbService {
        private readonly DbRepository _dbRepository;
        public DbService(DbRepository dbRepository) { _dbRepository = dbRepository; }
        //
        public void InsertRow(DataTable dataTable) => _dbRepository.InsertRow(dataTable);
        public void DeleteRow(DataTable dataTable) => _dbRepository.DeleteRow(dataTable);
        public void UpdateRow(DataTable dataTable) => _dbRepository.UpdateRow(dataTable);
    }
}