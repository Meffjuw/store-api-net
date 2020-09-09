using System;

namespace Api.Entities
{
  public class Product
  {
    public int Id {get;set;}
    public bool Visible {get;set;}
    public string ProductId{get;set;}
    public string Name{get;set;}
    public DateTime? ReleaseDate{get;set;}
    public string VendorId{get;set;}
  }
}