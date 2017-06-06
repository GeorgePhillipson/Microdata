using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Archetype.Models;
using Microdata.Models;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Core;




namespace Microdata.Controllers
{
    public class HomeController : SurfaceController
    {
        private const string PartialViewFolder  = "~/Views/Partials/Archetype/";
        private const string PartialSubFolder   = "/ArchetypeHelpers/";
        public ActionResult Index()
        {
            IPublishedContent services = CurrentPage;
            ArchetypeModel service = services.GetPropertyValue<ArchetypeModel>("products");//Name given to Archetype in DocType 
            List<MicrodataModel> model = new List<MicrodataModel>();

            foreach (ArchetypeFieldsetModel content in service.Where(x => x.Alias == "displayDemoProductsArchetype")) //Name given to Archetype in 
            {
                foreach (ArchetypeFieldsetModel nestedContent in content.GetValue<ArchetypeModel>("productsForSale")) // Name given to nested Archetype in displayDemoProductsArchetype
                {
                    string title        = nestedContent.GetValue<string>("Title");
                    string description  = nestedContent.GetValue<string>("Description");
                    string currencyCode = nestedContent.GetValue<string>("CurrencyCode");
                    string price        = nestedContent.GetValue<string>("ItemPrice");

                    model.Add(new MicrodataModel(title, description, currencyCode, price));
                }
            }
                return PartialView($"{PartialViewFolder}{PartialSubFolder}pvProducts.cshtml", model);
        }


        public ActionResult CommentsForm()
        {
            
            return PartialView("~/Views/Partials/pvCommentsForm.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CommentsForm(CommentsModel model)
        {
            try
            {
            CreateBlogMessage(model);
            TempData["Message"] = "Your post has been received";
            return RedirectToCurrentUmbracoPage();
            }
            catch (Exception)
            {
                TempData["Message"] = "There was a problem submitting your post";
                return RedirectToCurrentUmbracoPage();
            }
        }

        private void CreateBlogMessage(CommentsModel model)
        {
                var contentService  = ApplicationContext.Current.Services.ContentService;
                var pageName        = CurrentPage.Name;
                var allEventsNode   = GetAllEventsNode();
                var content = contentService.CreateContent(pageName, allEventsNode.Id, "reviewComments");
                content.SetValue("messageTitle", model.MessageTitle);
                content.SetValue("visitorMessage", model.VisitorMessage);
                contentService.SaveAndPublishWithStatus(content);
        }

        private IPublishedContent GetAllEventsNode()
        {
            return Umbraco.TypedContentAtRoot().DescendantsOrSelf("reviewComments").FirstOrDefault();
        }

        [HttpGet]
        public ActionResult DisplayComments()
        {

            try
            {
                IEnumerable<IPublishedContent> posts = CurrentPage.AncestorOrSelf(1).DescendantsOrSelf("reviewComments").ToList();

                List<DisplayCommentsModel> model = new List<DisplayCommentsModel>();

                foreach (IPublishedContent content in posts.Where(x => x.GetPropertyValue<bool>("commentsReviewed")))
                {
                    string messageTitle     = content.GetPropertyValue<string>("messageTitle");
                    string visitorMessage   = content.GetPropertyValue<string>("visitorMessage");
                   
                    model.Add(new DisplayCommentsModel(messageTitle, Regex.Replace(visitorMessage, Environment.NewLine, "<br/>")));
                }

                return PartialView("~/Views/Partials/pvDisplayComments.cshtml", model);
            }
            catch (Exception)
            {
                TempData["Message"] = "There was a problem displaying posts";
                return CurrentUmbracoPage();
            }
        }
    }
}