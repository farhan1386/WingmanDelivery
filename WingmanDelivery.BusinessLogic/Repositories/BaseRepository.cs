using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using WingmanDelivery.BusinessLogic.UnitOfWork;

namespace WingmanDelivery.BusinessLogic.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected IDbConnection _connection => UnitOfWork.Connection;
        protected IDbTransaction? _transaction => UnitOfWork.Transaction;
        protected Guid _userUid => UnitOfWork.Data.userUid;
        protected string _schema => UnitOfWork.Data.schema;
        protected int? _commandTimeout => UnitOfWork.Data.commandTimeout;

        protected BaseRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
    }
}
