using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;

namespace Scheduler
{
    internal class DynamoDB
    {
        #region Properties

        public List<TaskConfig> TasksDefinition { get; private set; }
        private Table Table { get; }
        private AmazonDynamoDBClient AWSDbClient { get; }

        #endregion Properties

        #region Constructors

        public DynamoDB(AWSCredentials credentials, string tableName)
        {
            this.AWSDbClient = new AmazonDynamoDBClient(credentials);
            this.Table = Table.LoadTable(AWSDbClient, tableName);
        }

        public DynamoDB(string tableName)
        {
            string strAccessKey = "ASIAVNSXWPJRPTF7ZNFT";
            string strSecretKey = "E9l5lNZiO7PYcbBOS+oU2DL985NhRLjiZWwgctu5";
            string strAccessToken = "IQoJb3JpZ2luX2VjELX//////////wEaCXVzLWVhc3QtMSJIMEYCIQCuwmxuvRl2FUTDL5wnCkyOulxVIh7IplzUdDTbOlwrRwIhAPRtwsy07fW+ZmdfEFTBolEHlW6SiY7aADPn7ZqvtO+SKo4DCD0QARoMMzcyNzcyMzM4Mjc0Igx6js5Cr0Hp+qNcIq8q6wJ4TfwNrUfNgvmnWcCgWFbP2qCII/JUAvP2gj8M1aiLEJCweDzZWH+V980qfF4QS9DA7oA8dIvm4F/6ndzPtl8PrU4q3uKxy5djkgH0qUK+6AKykSSQPH48PKrXhwyi7N4TtRy5kRhvBnAQsfF9QRLsLAINuGPATK79fWsINrgqG5oG6LlkS9SCsWIw+nRXLL/s4sWVQKruwZKPQhjlagPfdR45+JbZeutEkvej/iL7dhPHqPJGOcAFbTuJx//1UHbgYSirMHYu0aDrnaKzLTo1hiA+L5XGRrp/kpW9Ps17PP5970jbOsqo5Kn+nv3xM1RH9EBz53T+jIR5eYXK7J/1dig2RWi4pr/OnqcVt0ERsRuqfVkwGwadxUvd63IZJZTWJqh5aTsIsXuHgWKqZt+TFw0hTtlloMXg95PiH4syTEQIe8RxT3jGXHPJ1aVVwuTXOMn+GgZ/QU+dFA63pBQScad4t625RqDpog8w+7nqkQY6pQHxp8R2OnVV9MBuYo0k14KQ9in/4DkcRbKOuFBWAA1ebS9FcElu8y+mNziUE8hUzX7dxgHQjIyNIw+WCTzDdNznNVorlS/AaH9+d7xBcgfU9RamG8zS+BinZG8450Ys4Qe8Oc33AXePuIjxmtruTL/auf2spUARjfPcYXUagKW8xN0FU8TK3wReCljh7l+OeMbWJjb6zhGdfoEw4JGAM1bdiSLa6hY=";
            string strRegion = "us-east-1";

            this.AWSDbClient = new AmazonDynamoDBClient(strAccessKey, strSecretKey, strAccessToken, Amazon.RegionEndpoint.GetBySystemName(strRegion));
            this.Table = Table.LoadTable(AWSDbClient, tableName);
        }

        #endregion Constructors

        #region Methods

        public List<TaskConfig> GetItemsByFilter(DynamoItemFilterList taskDefinitionFilter)
        {
            this.GetItemsAsync(taskDefinitionFilter).GetAwaiter().GetResult();

            return TasksDefinition;
        }

        private async System.Threading.Tasks.Task GetItemsAsync(DynamoItemFilterList taskDefinitionFilterList)
        {
            ScanFilter scanFilter = new ScanFilter();
            foreach (DynamoItemFilter taskDefinitionFilter in taskDefinitionFilterList.ItemFilters)
            {
                scanFilter.AddCondition(taskDefinitionFilter.Attibute, taskDefinitionFilter.Operator, taskDefinitionFilter.Value);
            }

            Search search = Table.Scan(scanFilter);

            List<List<Document>> docsList = new List<List<Document>>();

            do
            {
                try
                {
                    var getNextBatch = search.GetNextSetAsync();
                    var docList = await getNextBatch;
                    docsList.Add(docList);
                }
                catch (Exception e)
                {
                    Console.WriteLine("[ERROR] " + e.Message);
                }
            } while (!search.IsDone);

            this.ParseDynamoResult(docsList);
        }

        private void ParseDynamoResult(List<List<Document>> dynamoResult)
        {
            List<TaskConfig> tasksDefinition = new List<TaskConfig>();

            foreach (var tasks in dynamoResult)
            {
                foreach (var task in tasks)
                {
                    var AgentConfig = new TaskConfig().GetFromDynamoDB(task);
                    tasksDefinition.Add(AgentConfig);
                }
            }

            this.TasksDefinition = tasksDefinition;
        }

        public void UpdateSchedulingStatus(List<TaskConfig> tasksConfig)
        {
        }

        #endregion Methods
    }
}