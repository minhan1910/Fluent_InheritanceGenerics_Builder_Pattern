using System.Text;
using static System.Console;

namespace BuilderPatternDemo
{
    #region Fluent Builder
    public class HtmlElement
    {
        public string Name { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public List<HtmlElement> Elements { get; set; } = new List<HtmlElement>();
        private const int INDENT_SIZE = 2;

        public HtmlElement() { }

        public HtmlElement(string name, string text)
        {
            Name = name;
            Text = text;
        }

        private string MakeIndent(int levelIndent)
        {
            return new string(' ', INDENT_SIZE * levelIndent);
        }

        public string ToStringImpl(int indent)
        {
            var sb = new StringBuilder();
            var i = MakeIndent(indent);
            sb.AppendLine($"{i}<{Name}>");

            if (!string.IsNullOrWhiteSpace(Text))
            {
                sb.Append(MakeIndent(indent + 1));
                sb.AppendLine(Text);
            }

            foreach (var element in Elements)
            {
                sb.Append(element.ToStringImpl(indent + 1));
            }

            sb.AppendLine($"{i}</{Name}>");

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToStringImpl(0);
        }
    }

    public class HtmlBuilder
    {
        private readonly string rootName;
        private HtmlElement Root { get; set; } = new();

        public HtmlBuilder(string rootName)
        {
            this.rootName = rootName;
            Root.Name = rootName;
        }

        public HtmlBuilder AddChild(string childName, string childText)
        {
            var e = new HtmlElement(childName, childText);
            Root.Elements.Add(e);
            return this;
        }

        public override string ToString()
        {
            return Root.ToString();
        }

        public void Clear()
        {
            Root = new HtmlElement
            {
                Name = rootName,
            };
        }
    }

    #endregion

    #region Fluent Builder Inheritance With Recursive Generics

    public class Person
    {
        public string? Name { get; set; }
        public string? Position { get; set; }
        public class Builder : PersonJobBuilder<Builder> {}
        public static Builder New => new Builder();
        public override string? ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Position)}: {Position}";
        }
    }

    public abstract class PersonBuilder
    {
        protected Person person = new();

        public Person Build()
        {
            return person;
        }
    }

    // SELF is prefer the object inherit object from object
    // class Foo : Bar<Foo>
    public class PersonInfoBuilder<SELF> : PersonBuilder
        where SELF : PersonInfoBuilder<SELF>
    {
        //protected Person person = new();

        public SELF /*PersonInfoBuilder*/ Called(string name)
        {
            WriteLine(nameof(PersonInfoBuilder<SELF>) + " class - SELF: " + typeof(SELF).Name);
            person.Name = name;
            return this as SELF;
        }
    }

    /*
        Imagine you have a requirement for adding for more responsisbiliy for PersonInforBuilder
        remains applying Open-Close Principle.

    
            public PersonInfoBuilder Called(string name)
            {            
                person.Name = name;
                return this;
            }

             public class PersonJobBuilder : PersonInfoBuilder
            {
                ****
                * The problem is when you call new PersonJobBuilder().Called().WorkAsA() 
                * (WordAsA method is not in PersonInfoBuilder -> the main problem) 
                ****
                public PersonJobBuilder WorkAsA(string position)
                {
                    person.Position = position;
                    return this;
                }
            }

        The solution for this problem can be solved by using base class and the recursive generics technique

     */
    public class PersonJobBuilder<SELF> : PersonInfoBuilder<SELF>
        where SELF : PersonJobBuilder<SELF>
    {
        public SELF /*PersonJobBuilder*/ WorkAsA(string position)
        {
            WriteLine(nameof(PersonJobBuilder<SELF>) + " class - SELF: " + typeof(SELF).Name);
            person.Position = position;
            return this as SELF;
        }
    }

    public static class TestBuilderInheritanceGenerics
    {
        public static void Test()
        {
            var me = Person.New
                        .Called("MyName")
                        .WorkAsA("Dev")
                        .Build();

            WriteLine(me);

            // explicit type
            PersonJobBuilder<Person.Builder> builder = Person.New
                .Called("MyName")
                .WorkAsA("Dev");
        }
    }

    #endregion

    internal class Program
    {
        static void WithoutBuilderPattern()
        {
            var hello = "Hello";
            var sb = new StringBuilder();
            sb.Append("<p>");
            sb.Append(hello);
            sb.Append("</p>");
            WriteLine(sb.ToString());

            var words = new[] { "Hello", "World" };
            sb.Clear();
            sb.Append("<ul>\n");

            foreach (var word in words)
            {
                sb.AppendFormat("\t<li>{0}</li>\n", word);
            }
            
            sb.Append("</ul>");
            
            WriteLine(sb);
        }

        static void TestWithHtmlElementBuilder()
        {
            var builder = new HtmlBuilder("ul");

            // Fluent Builder
            builder.AddChild("li", "hello")
                   .AddChild("li", "world");

            WriteLine(builder);
        }

        static void Main(string[] args)
        {
            //var builder = new PersonJobBuilder();
            //builder.Called("MyName")
            //    .WorkAsA();

            //new PersonInfoBuilder<???> can not use it directly

          

        }
    }
}