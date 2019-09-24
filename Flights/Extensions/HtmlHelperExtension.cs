using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Flights.Extensions
{
    public class HtmlBuilder<TModel>
    {
        public readonly IHtmlHelper<TModel> helper;

        public HtmlBuilder(IHtmlHelper<TModel> helper)
        {
            this.helper = helper;
        }

        public TagBuilder FormTextBoxFor<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            TagBuilder tag = new TagBuilder("div");
            tag.AddCssClass("form-group");
            tag.InnerHtml.AppendHtml(this.helper.LabelFor(expression));
            tag.InnerHtml.AppendHtml(this.helper.TextBoxFor(expression, new { @class = "form-textbox" }));
            tag.InnerHtml.AppendHtml(this.helper.ValidationMessageFor(expression));
            return tag;
        }

        public TagBuilder FormAreaFor<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            TagBuilder tag = new TagBuilder("div");
            tag.AddCssClass("form-group");
            tag.InnerHtml.AppendHtml(this.helper.LabelFor(expression));
            tag.InnerHtml.AppendHtml(this.helper.TextAreaFor(expression, new { @class = "form-textbox" }));
            tag.InnerHtml.AppendHtml(this.helper.ValidationMessageFor(expression));
            return tag;
        }

        public TagBuilder FormEditorFor<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            TagBuilder tag = new TagBuilder("div");
            tag.AddCssClass("form-group");
            tag.InnerHtml.AppendHtml(this.helper.LabelFor(expression));
            tag.InnerHtml.AppendHtml(this.helper.EditorFor(expression, new { @class = "form-control" }));
            tag.InnerHtml.AppendHtml(this.helper.ValidationMessageFor(expression));
            return tag;
        }

        public TagBuilder FormCheckBoxFor(Expression<Func<TModel, bool>> expression)
        {
            TagBuilder tag = new TagBuilder("div");
            tag.AddCssClass("form-group");
            tag.InnerHtml.AppendHtml(this.helper.LabelFor(expression));
            tag.InnerHtml.AppendHtml(this.helper.CheckBoxFor(expression, new { @style = "margin-left:0.5em" }));
            tag.InnerHtml.AppendHtml(this.helper.ValidationMessageFor(expression));
            return tag;
        }

        public TagBuilder FormDatePickerFor<TProperty>(Expression<Func<TModel, TProperty>> expression, string id)
        {
            TagBuilder tag = new TagBuilder("div");
            tag.AddCssClass("form-group");
            tag.InnerHtml.AppendHtml(this.helper.LabelFor(expression));

            TagBuilder InputGroup = new TagBuilder("div");
            InputGroup.AddCssClass("input-group date");

            TagBuilder InputGroupAddon = new TagBuilder("div");
            InputGroupAddon.AddCssClass("input-group-addon");

            TagBuilder CalendarIcon = new TagBuilder("i");
            CalendarIcon.AddCssClass("fa fa-calendar");
            InputGroupAddon.InnerHtml.AppendHtml(CalendarIcon);
            InputGroup.InnerHtml.AppendHtml(InputGroupAddon);
            InputGroup.InnerHtml.AppendHtml(this.helper.TextBoxFor(expression, new { @class = "form-control pull-right", @id = id }));
            tag.InnerHtml.AppendHtml(InputGroup);
            tag.InnerHtml.AppendHtml(this.helper.ValidationMessageFor(expression));
            return tag;
        }

        public TagBuilder FormTextBoxFor<TProperty>(Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            TagBuilder tag = new TagBuilder("div");
            IDictionary<string, object> Attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            Attributes.Add("class", "form-textbox");

            tag.AddCssClass("form-group");
            tag.InnerHtml.AppendHtml(this.helper.LabelFor(expression));
            tag.InnerHtml.AppendHtml(this.helper.TextBoxFor(expression, Attributes));
            tag.InnerHtml.AppendHtml(this.helper.ValidationMessageFor(expression));
            return tag;
        }

        public TagBuilder FormTextBoxFor<TProperty>(Expression<Func<TModel, TProperty>> expression, object value, string format, object htmlAttributes)
        {
            TagBuilder tag = new TagBuilder("div");
            IDictionary<string, object> Attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            Attributes.Add("class", "form-textbox");

            tag.AddCssClass("form-group");
            tag.InnerHtml.AppendHtml(this.helper.LabelFor(expression));
            tag.InnerHtml.AppendHtml(this.helper.TextBox(((MemberExpression)expression.Body).Member.Name, value, format, Attributes));
            tag.InnerHtml.AppendHtml(this.helper.ValidationMessageFor(expression));
            return tag;
        }

        public TagBuilder FormPasswordBoxFor<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            TagBuilder tag = new TagBuilder("div");
            tag.AddCssClass("form-group");
            tag.InnerHtml.AppendHtml(this.helper.LabelFor(expression));
            tag.InnerHtml.AppendHtml(this.helper.PasswordFor(expression, new { @class = "form-textbox" }));
            tag.InnerHtml.AppendHtml(this.helper.ValidationMessageFor(expression));
            return tag;
        }

        public TagBuilder FormDropDownListFor<TProperty>(Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel)
        {
            TagBuilder tag = new TagBuilder("div");
            tag.AddCssClass("form-group");
            tag.InnerHtml.AppendHtml(this.helper.LabelFor(expression));
            tag.InnerHtml.AppendHtml(this.helper.DropDownListFor(expression, selectList, optionLabel, new { @class = "form-control" }));
            tag.InnerHtml.AppendHtml(this.helper.ValidationMessageFor(expression));
            return tag;
        }

        public TagBuilder FormListBoxFor<TProperty>(Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
        {
            TagBuilder tag = new TagBuilder("div");
            tag.AddCssClass("form-group");
            tag.InnerHtml.AppendHtml(this.helper.LabelFor(expression));
            tag.InnerHtml.AppendHtml(this.helper.ListBoxFor(expression, selectList, new { @class = "form-control" }));
            tag.InnerHtml.AppendHtml(this.helper.ValidationMessageFor(expression));
            return tag;
        }

        public TagBuilder FormTransferListBoxFor<TProperty>(Expression<Func<TModel, TProperty>> expression, string firstListBoxName, IEnumerable<SelectListItem> selectList, string LabelSecondListBox, string secondListBoxName, IEnumerable<SelectListItem> secondSelectList)
        {
            TagBuilder tag = new TagBuilder("div");
            TagBuilder FirstListBox = new TagBuilder("div");
            FirstListBox.AddCssClass("col-md-2");
            FirstListBox.InnerHtml.AppendHtml(this.helper.LabelFor(expression));
            FirstListBox.InnerHtml.AppendHtml(this.helper.ListBox(firstListBoxName, selectList, new { @class = "form-control" }));
            FirstListBox.InnerHtml.AppendHtml(this.helper.ValidationMessageFor(expression));

            TagBuilder Buttons = new TagBuilder("div");
            Buttons.AddCssClass("listbox-transfer");
            Buttons.InnerHtml.AppendHtml("<button type='button' id='to-right'>></button>");
            Buttons.InnerHtml.AppendHtml("<button type='button' id='to-left'><</button>");

            TagBuilder SecondListBox = new TagBuilder("div");
            SecondListBox.AddCssClass("col-md-2");
            SecondListBox.InnerHtml.AppendHtml(this.helper.Label(LabelSecondListBox));
            SecondListBox.InnerHtml.AppendHtml(this.helper.ListBox(secondListBoxName, secondSelectList, new { @class = "form-control" }));

            tag.InnerHtml.AppendHtml(FirstListBox);
            tag.InnerHtml.AppendHtml(Buttons);
            tag.InnerHtml.AppendHtml(SecondListBox);

            TagBuilder HiddenList = new TagBuilder("div");
            HiddenList.AddCssClass("hidden");
            HiddenList.MergeAttribute("id", "hiddenList");

            tag.InnerHtml.AppendHtml(HiddenList);

            return tag;
        }

        public TagBuilder FormInputMaskFor<TProperty>(Expression<Func<TModel, TProperty>> expression, string InputMask)
        {
            TagBuilder tag = new TagBuilder("div");
            tag.AddCssClass("form-group");
            tag.InnerHtml.AppendHtml(this.helper.LabelFor(expression));
            tag.InnerHtml.AppendHtml(this.helper.TextBoxFor(expression, new { @class = "form-textbox" , inputmask = InputMask }));
            tag.InnerHtml.AppendHtml(this.helper.ValidationMessageFor(expression));
            return tag;
        }

        public string GetDisplayNameForEnum(Enum Enumerator)
        {
            return ((DisplayAttribute)Enumerator.GetType().GetMember(Enumerator.ToString()).First()?.GetCustomAttributes(typeof(DisplayAttribute), false).First())?.Name;
        }

        private static string GetString(IHtmlContent content)
        {
            var writer = new System.IO.StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }
    }

    public static class HtmlHelperExtension
    {
        public static HtmlBuilder<TModel> Flights<TModel>(this IHtmlHelper<TModel> helper)
        {
            return new HtmlBuilder<TModel>(helper);
        }
    }
}
