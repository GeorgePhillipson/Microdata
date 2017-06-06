using System.ComponentModel;

namespace Microdata.Models
{
    public class MicrodataModel
    {
        public MicrodataModel(string title, string description, string currencyCode, string itemPrice)
        {
            Title           = title;
            Description     = description;
            CurrencyCode    = currencyCode;
            ItemPrice       = itemPrice;
        }

        public string Title         { get; set; }
        public string Description   { get; set; }
        public string CurrencyCode  { get; set; }
        public string ItemPrice     { get; set; }
    }

    public abstract class CommentFormModel
    {
        [DisplayName("Title")]
        public string MessageTitle { get; set; }
        [DisplayName("Message")]
        public string VisitorMessage { get; set; }
    }

    public class CommentsModel : CommentFormModel
    {
  
    }

    public class DisplayCommentsModel : CommentFormModel
    {
        public DisplayCommentsModel(string title, string message)
        {
            MessageTitle    = title;
            VisitorMessage  = message;
        }
    }
}