using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;


namespace FamilyDiagram.Model
{
    public class FamilyTreeModel
    {
        //public Target Target { get; set; } = new Target();
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public virtual Target Target { get; set; }//= new Target();
        //public string Notes { get; set; } = "";
    }

    /**
     * 人の基底クラス.
     * 基本的なプロパティはこのクラスにて保持する.
     * サブクラスに特有のプロパティはそちらで保持する.
     */
    public class Person
    {
        public int Layer { get; set; } = 0;
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FullName { get; set; } = "";
        //public bool Dead { get; set; } = false;
        virtual public string Relation { get; set; } = "";
        //public DateTime BirthDay { get; set; } = Common.INITIAL_DATE;

        // CAVさんへ：不足しているプロパティがある場合は追加して構いません
    }

    /**
     * 被相続人を表現するクラス.
     */
    public class Target : Person, HasAncestor, HasFamilies
    {
        override public string Relation
        {
            get { return "被相続人"; }
        }
        [ForeignKey("Ancestor1Id")]
        public virtual Ancestor? Ancestor1 { get; set; }// = new Ancestor();
        [ForeignKey("Ancestor2Id")]
        public virtual Ancestor? Ancestor2 { get; set; }// = new Ancestor();
        public virtual List<Family> Families { get; set; } //= new List<Family>();
    }

    /**
     * nullを許容する場合に使うクラス.
     * 逆に言えば、このクラスを使わない場合はnullを想定しなくてよいし、また
     * 想定させてもならない.
     */
    public class Nullable<T>
    {
        public required T Value { get; set; }
    }

    /**
     * 被相続人から見た祖先側の人物.
     * 祖先は、更に祖先を持つ可能性がある.
     */
    public class Ancestor : Person, HasAncestor
    {
        [ForeignKey("Ancestor1Id")]
        public virtual Ancestor? Ancestor1 { get; set; }
        [ForeignKey("Ancestor2Id")]
        public virtual Ancestor? Ancestor2 { get; set; }
    }

    /**
     * 被相続人から見た配偶者.
     */
    public class Spouse : Person, HasFamilies
    {
        public DateTime Start { get; set; } = Common.INITIAL_DATE;
        public DateTime End { get; set; } = Common.INITIAL_DATE;
        public virtual List<Family>? Families { get; set; }
    }

    /**
     * 被相続人から見て子供側の人物.
     * 子供は更に子供がいる可能性がある.
     */
    public class Child : Person, HasFamilies
    {
        public virtual List<Family>? Families { get; set; }// = new List<Family>();
    }

    /**
     * 子供がいるということは配偶者が必ずいる.
     * それらのペアを「家族」として管理する.
     */
    public class Family
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("PersonId")]
        public virtual Spouse? Spouse { get; set; }// = new Spouse();
        public virtual List<Child>? Children { get; set; }// = new List<Child>();
    }

    /**
     * 親がいることを示す.
     * 戸籍上の表現ではなく、あくまでモデル上での表現.
     */
    public interface HasAncestor
    {
        Ancestor Ancestor1 { get; set; }
        Ancestor Ancestor2 { get; set; }
    }

    /**
     * 家族があることを示す.
     * 戸籍上の表現ではなく、あくまでモデル上での表現.
     */
    public interface HasFamilies
    {
        public List<Family> Families { get; set; }
    }

    public static class Common
    {
        internal static readonly DateTime INITIAL_DATE = new DateTime(1900, 1, 1);

        public static T CopyPerson<T>(T originalPerson) where T : Person, new()
        {
            if (originalPerson == null)
            {
                throw new ArgumentNullException(nameof(originalPerson));
            }

            // Start by copying the base attributes common to all Person classes
            T copiedPerson = new T
            {
                Id = Guid.NewGuid().ToString(),  // Generate a new unique ID
                FullName = originalPerson.FullName,
                //Dead = originalPerson.Dead,
                Relation = originalPerson.Relation,
                //BirthDay = originalPerson.BirthDay
            };

            // Now, check for additional properties based on the derived type
            if (copiedPerson is Target targetPerson && originalPerson is Target originalTarget)
            {
                // Handle Target-specific properties
                targetPerson.Families = originalTarget.Families; // Assuming it's a shallow copy
                targetPerson.Ancestor1 = originalTarget.Ancestor1;
                targetPerson.Ancestor2 = originalTarget.Ancestor2;
            }
            else if (copiedPerson is Ancestor ancestorPerson && originalPerson is Ancestor originalAncestor)
            {
                // Handle Ancestor-specific properties
                ancestorPerson.Ancestor1 = originalAncestor.Ancestor1;
                ancestorPerson.Ancestor2 = originalAncestor.Ancestor2;
            }
            else if (copiedPerson is Spouse spousePerson && originalPerson is Spouse originalSpouse)
            {
                // Handle Spouse-specific properties
                spousePerson.Start = originalSpouse.Start;
                spousePerson.End = originalSpouse.End;
            }

            return copiedPerson;
        }

    }
}