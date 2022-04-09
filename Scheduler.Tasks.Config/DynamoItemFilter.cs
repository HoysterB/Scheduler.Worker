using Amazon.DynamoDBv2.DocumentModel;

namespace Scheduler.Core
{
    public class DynamoItemFilterList
    {
        #region
        private List<DynamoItemFilter> itemFilters;

        public List<DynamoItemFilter> ItemFilters
        {
            get
            {
                if (itemFilters == null)
                    this.itemFilters = new List<DynamoItemFilter>();
                return itemFilters;
            }
        }

        #endregion

        public void Add(DynamoItemFilter taskDefinitionFilter)
        {
            this.ItemFilters.Add(taskDefinitionFilter);
        }
    }

    public class DynamoItemFilter
    {
        public string Attibute { get; }
        public ScanOperator Operator { get; }
        public DynamoDBEntry Value { get; }

        public DynamoItemFilter(string attibute, ScanOperator scanOperator, DynamoDBEntry value)
        {
            this.Attibute = attibute;
            this.Operator = scanOperator;
            this.Value = value;
        }
    }
}