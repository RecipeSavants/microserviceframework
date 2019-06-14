public class QuestionModel
{
  public QuestionVertex Question {get;set;}
  public List<AnswerVertex> Answers {get;set;}
  
  public QuestionModel()
  {
    Question = new QuestionVertex();
    Answers = new List<AnswerVertex>();
  }
}
