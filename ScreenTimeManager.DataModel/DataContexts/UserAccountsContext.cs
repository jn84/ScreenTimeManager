using System.Data.Entity;


namespace ScreenTimeManager.DataModel.DataContexts
{
	public class UserAccountsContext : DbContext
	{
		//DbSet<>
		// DbSet

		// override SaveChanges to persist certain data

		// This setup will not work
		////// It will require circular dependency
	}
}
