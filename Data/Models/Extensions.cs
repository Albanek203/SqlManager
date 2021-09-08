using System.Data;
using SqlManager.Data.Service;
using Microsoft.Extensions.DependencyInjection;

namespace SqlManager.Data.Models {
    public static class Extensions {
        public static void ExtensionAcceptChanges(this DataTable dataTable) {
            var dbService = App.ServiceProvider.GetService<DbService>();
            if (dbService != null) {
                dbService.InsertRow(dataTable);
                dbService.UpdateRow(dataTable);
                dbService.DeleteRow(dataTable);
            }
            dataTable.AcceptChanges();
        }
    }
}