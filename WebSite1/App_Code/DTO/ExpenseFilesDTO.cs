﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ExpenseFilesDTO
/// </summary>
public class ExpenseFilesDTO
{
	public int Id { get; set; }
	public Doc_Tools.DocumentType ExpenseType { get; set; }
	public int ExpenseId { get; set; }
	public int ExpenseDetailId { get; set; }
	public string FileName { get; set; }
	public FileType Type { get; set; }
	public string ContentType { get; set; }
	public byte[] FileBinary { get; set; }
	public int FileLength { get; set; }
	public DateTime DateCreated { get; set; }
	public ExpenseFilesDTO()
	{
		//
		// TODO: Add constructor logic here
		//
	}

	public enum FileType
    {
		Xml=1 , Pdf=2, Voucher=3
    }
}