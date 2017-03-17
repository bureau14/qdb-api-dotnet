using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.Native;

namespace Quasardb.Tests.Misc
{
    [TestClass]
    public class QdbExceptionFactoryTests
    {
        [TestMethod]
        public void qdb_e_alias_already_exists()
        {
            var exception = QdbExceptionFactory.Create(qdb_error_t.qdb_e_alias_already_exists);

            Assert.IsInstanceOfType(exception, typeof (QdbAliasAlreadyExistsException));
            Assert.IsInstanceOfType(exception, typeof(QdbOperationException));
        }

        [TestMethod]
        public void qdb_e_alias_not_found()
        {
            var exception = QdbExceptionFactory.Create(qdb_error_t.qdb_e_alias_not_found);

            Assert.IsInstanceOfType(exception, typeof(QdbAliasNotFoundException));
            Assert.IsInstanceOfType(exception, typeof(QdbOperationException));
        }

        [TestMethod]
        public void qdb_e_incompatible_type()
        {
            var exception = QdbExceptionFactory.Create(qdb_error_t.qdb_e_incompatible_type);

            Assert.IsInstanceOfType(exception, typeof(QdbIncompatibleTypeException));
            Assert.IsInstanceOfType(exception, typeof(QdbOperationException));
        }

        [TestMethod]
        public void qdb_e_invalid_argument()
        {
            var exception = QdbExceptionFactory.Create(qdb_error_t.qdb_e_invalid_argument);

            Assert.IsInstanceOfType(exception, typeof(QdbInvalidArgumentException));
            Assert.IsInstanceOfType(exception, typeof(QdbInputException));
        }

        [TestMethod]
        public void qdb_e_resource_locked()
        {
            var exception = QdbExceptionFactory.Create(qdb_error_t.qdb_e_resource_locked);

            Assert.IsInstanceOfType(exception, typeof(QdbResourceLockedException));
            Assert.IsInstanceOfType(exception, typeof(QdbOperationException));
        }

        [TestMethod]
        public void qdb_e_connection_refused()
        {
            var exception = QdbExceptionFactory.Create(qdb_error_t.qdb_e_connection_refused);

            Assert.IsInstanceOfType(exception, typeof(QdbConnectionException));
        }

        [TestMethod]
        public void qdb_e_invalid_reply()
        {
            var exception = QdbExceptionFactory.Create(qdb_error_t.qdb_e_invalid_reply);

            Assert.IsInstanceOfType(exception, typeof(QdbProtocolException));
        }

        [TestMethod]
        public void qdb_e_no_memory_local()
        {
            var exception = QdbExceptionFactory.Create(qdb_error_t.qdb_e_no_memory_local);

            Assert.IsInstanceOfType(exception, typeof(QdbLocalSystemException));
            Assert.IsInstanceOfType(exception, typeof(QdbSystemException));
        }

        [TestMethod]
        public void qdb_e_no_memory_remote()
        {
            var exception = QdbExceptionFactory.Create(qdb_error_t.qdb_e_no_memory_remote);

            Assert.IsInstanceOfType(exception, typeof(QdbRemoteSystemException));
            Assert.IsInstanceOfType(exception, typeof(QdbSystemException));
        }
    }
}
