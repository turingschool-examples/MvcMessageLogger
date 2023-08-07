namespace MvcMessageLogger.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Message> Messages { get; } = new List<Message>();

        public string Encrypt(string password)
        {
            var key = new Dictionary<char, int>
            {
                {'a', 1},
                {'b', 2},
                {'c', 3},
                {'d', 4},
                {'e', 5},
                {'f', 6},
                {'g', 7},
                {'h', 8},
                {'i', 9},
                {'j', 10},
                {'k', 11},
                {'l', 12},
                {'m', 13},
                {'n', 14},
                {'o', 15},
                {'p', 16},
                {'q', 17},
                {'r', 18},
                {'s', 19},
                {'t', 20},
                {'u', 21},
                {'v', 22},
                {'w', 23},
                {'x', 24},
                {'y', 25},
                {'z', 26},
                {'1', 27},
                {'2', 28},
                {'3', 29},
                {'4', 30},
                {'5', 31},
                {'6', 32},
                {'7', 33},
                {'8', 34},
                {'9', 35},
                {'0', 36},
                {'!', 37},
                {'@', 38},
                {'#', 39},
                {'$', 40},
                {'%', 41},
                {'^', 42},
                {'&', 43},
                {'*', 44},
                {'(', 45},
                {')', 46},
                {'-', 47},
                {'_', 48},
                {'=', 49},
                {'+', 50}
            };
            int length = password.Length;
            int sum = 0;
            for(int i = 0; i < length; i++)
            {
                sum += i * (key[password[i]]);
            }
            return sum.ToString();
        }
    }
}
