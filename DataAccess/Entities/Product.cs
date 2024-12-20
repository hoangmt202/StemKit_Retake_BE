﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public string Description { get; set; }

    public string ImagePath { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public string Ages { get; set; }

    public int LabId { get; set; }

    public int SubcategoryId { get; set; }

    public virtual Lab Lab { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Subcategory Subcategory { get; set; }
}