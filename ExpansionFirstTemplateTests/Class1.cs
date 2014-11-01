using System;
using System.ComponentModel;

namespace ExpansionFirstExample
{
   public sealed class Customer : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;

      private string firstName;
      public string FirstName
      {
         get { return firstName; }
         set { SetProperty(ref firstName, value); }
      }

      private string lastName;
      public string LastName
      {
         get { return lastName; }
         set { SetProperty(ref lastName, value); }
      }

      private int id;
      public int Id
      {
         get { return id; }
         set { SetProperty(ref id, value); }
      }

      private DateTime birthDate;
      public DateTime BirthDate
      {
         get { return birthDate; }
         set { SetProperty(ref birthDate, value); }
      }
   }
}
