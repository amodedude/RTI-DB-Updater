using RTI.DataBase.Interfaces;
using RTI.DataBase.Interfaces.Repositories;
using RTI.DataBase.Model.Repositories;

namespace RTI.DataBase.Model
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RtiContext _context;

        public UnitOfWork(RtiContext context = null)
        {
            _context = context ?? new RtiContext();
            Sources = new SourceRepository(_context);
            WaterData = new WaterDataRepository(_context);
        }

        public IWaterDataRepository WaterData { get; private set; }
        public ISourceRepository Sources { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
