namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var key = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Guid.NewGuid().ToString("N");
            Console.WriteLine(key);
        }
    }
}
