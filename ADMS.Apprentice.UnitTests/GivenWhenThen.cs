using System.Linq;
using Adms.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace ADMS.Apprentice.UnitTests
{
	[TestClass]
	public abstract class GivenWhenThen
	{
		/// <summary>
		/// Steps that are run before each test.
		/// </summary>
		[TestInitialize]
		public void TestInitialize()
		{
			Given();
			When();
		}

		/// <summary>
		/// Sets up the environment for a specification context.
		/// </summary>
		protected virtual void Given()
		{
		}

		/// <summary>
		/// Acts on the context to create the observable condition.
		/// </summary>
		protected virtual void When()
		{
		}
	}

    [TestClass]
    public abstract class GivenWhenThen<T> where T : class
    {
        protected AutoMocker Container { get; private set; }
        protected T ClassUnderTest { get; private set; }

        [TestInitialize]
        public void SetUp()
        {
            Container = new AutoMocker();
            ClassUnderTest = Container.CreateInstance<T>();
            Given();
            When();
        }

        protected void StubRepositoryGet<TE>(TE entity) where TE : class, IAmAnAggregateRoot<int>
        {
            Container.GetMock<IRepository>().Setup(r => r.Get<TE>(entity.Id)).Returns(entity);
        }

        protected IQueryable<TE> StubRepositoryRetrieve<TE>(params TE[] entitiesToReturn) where TE : class, IAmAnAggregateRoot
        {
            IQueryable<TE> results = entitiesToReturn.AsQueryable();
            Container.GetMock<IRepository>().Setup(r => r.Retrieve<TE>()).Returns(results);
            return results;
        }

        protected virtual void Given()
        {
        }

        protected virtual void When()
        {
        }

    }

}