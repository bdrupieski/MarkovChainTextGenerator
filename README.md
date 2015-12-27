This is a Markov Chain text generator I wrote for fun from scratch one night. I wrote it primarily to generate sentences from posts scraped from the Something Awful forums.

The design is partially inspired by [this code](https://github.com/jt-halbert/spark-workshop/blob/master/followalong-20151206.scala) from an Apache Spark workshop I attended.

The code to train a model on text and generate new sentences is in the project MarkovChainTextGeneratorModel.

The project SomethingAwfulTextGenerator scrapes specific Something Awful forum threads and trains a model on the scraped posts, then generates a file with new sentences generated from those posts. To use SomethingAwfulTextGenerator you will need to supply your own cookies in order to successfully download any HTML. The cookies sit in a git-ignored .json file in this project. Add a file to the project that looks like this with your own cookie values:

```json
{
  "__cfduid": "your",
  "bbuserid": "cookie",
  "bbpassword": "values",
  "aduserid": "go",
  "__csdrc": "in",
  "sessionid": "these",
  "sessionhash": "properties"
}
```
