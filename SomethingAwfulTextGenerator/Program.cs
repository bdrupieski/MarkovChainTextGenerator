using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MarkovChainTextGeneratorModel;
using Newtonsoft.Json;
using NSoup;

namespace SomethingAwfulTextGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            int threadId = -1;
            int userId = -1;

            var filename = $"t{threadId}u{userId}.json";
            List<Post> posts;

            if (File.Exists(filename))
            {
                var json = File.ReadAllText(filename);
                posts = JsonConvert.DeserializeObject<List<Post>>(json);
            }
            else
            {
                var cookies = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("forums-cookies.json"));
                posts = RetrievePosts(threadId, userId, cookies);
                var json = JsonConvert.SerializeObject(posts, Formatting.Indented);
                File.WriteAllText(filename, json);
            }

            var model = Model.Build(posts.Select(x => x.Text));

            Console.WriteLine("Most likely sentence:");
            Console.WriteLine(model.GenerateMostLikelySentence());
            Console.WriteLine();

            Console.WriteLine("Probable sentences:");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(model.GenerateProbableSentence());
            }
            Console.WriteLine();

            Console.WriteLine("Random sentences:");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(model.GenerateRandomSentence());
            }
            Console.WriteLine();

            var probableSentencesFilename = $"t{threadId}u{userId}_prob_sentences.txt";
            var probableSentences = Enumerable.Range(0, 10000).Select(x => model.GenerateProbableSentence());
            File.WriteAllLines(probableSentencesFilename, probableSentences);
        }

        public static List<Post> RetrievePosts(int threadId, int userId, Dictionary<string, string> cookies)
        {
            var posts = new List<Post>();

            int page = 1;
            bool hasAdditionalPages = true;
            while (hasAdditionalPages)
            {
                var urlFormat = "http://forums.somethingawful.com/showthread.php?threadid={0}&userid={1}&perpage=40&pagenumber={2}";
                var html = NSoupClient.Connect(string.Format(urlFormat, threadId, userId, page))
                                      .Timeout(5000)
                                      .Cookies(cookies)
                                      .Get();

                foreach (var postHtml in html.Select(".post"))
                {
                    var body = postHtml.Select(".postbody");
                    body.Select(".editedBy").Remove();
                    body.Select(".bbc-block").Remove();

                    posts.Add(new Post(postHtml.Id, body.Text));
                }

                hasAdditionalPages = html.GetElementsByAttributeValue("title", "Last page").Any();
                page++;
                Thread.Sleep(4000);
            }

            return posts;
        }

        public class Post
        {
            public Post(string id, string text)
            {
                Id = id;
                Text = text;
            }

            public string Id { get; }
            public string Text { get; }

            public override string ToString()
            {
                return $"Text: {Text}";
            }
        }
    }
}
