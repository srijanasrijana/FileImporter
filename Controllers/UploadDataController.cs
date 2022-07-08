using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UserManagementApp.Models;

namespace UserManagementApp.Controllers
{
    [Authorize]
    public class UploadDataController : Controller
    {

        private UsersManagementEntities db = new UsersManagementEntities();

        public ActionResult Create()
        {
            return View(new List<UploadDetail>());
        }

        [HttpPost]
        public ActionResult Create(HttpPostedFileBase postedFile)
        {
            List<UploadDetail> uploadData = new List<UploadDetail>();

            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                //Create a DataTable.
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[7] {
                                new DataColumn("TerminalId", typeof(int)),
                                new DataColumn("SerialNo", typeof(int)),
                                new DataColumn("MerchantId", typeof(int)),
                                new DataColumn("Description",typeof(string)),
                                new DataColumn("Manufacturer",typeof(string)),
                                new DataColumn("Model",typeof(string)),
                                new DataColumn("State",typeof(string))
                });


                //Read the contents of CSV file.
                string csvData = System.IO.File.ReadAllText(filePath);

                String[] rows = csvData.Split('\n');
                int len = rows.Length;
                if (len > 1)
                {
                    //Execute a loop over the rows.
                    for(int i=0; i < len; i++)
                    {
                        // Skip header
                        if (i == 0) continue;

                        String row = rows[i];
                        if (!string.IsNullOrEmpty(row))
                        {
                            dt.Rows.Add();
                            int j = 0;

                            String[] cells = row.Split(',');

                            //Execute a loop over the columns and prepare datatable
                            foreach (string cell in cells)
                            {
                                dt.Rows[dt.Rows.Count - 1][j] = cell;
                                j++;
                            }

                            //Add into return list
                            uploadData.Add(new UploadDetail
                            {
                                TerminalId = Convert.ToInt32(row.Split(',')[0]),
                                SerialNo = Convert.ToInt32(row.Split(',')[1]),
                                MerchantId = Convert.ToInt32(row.Split(',')[2]),
                                Description = row.Split(',')[3],
                                Manufacturer = row.Split(',')[4],
                                Model = row.Split(',')[5],
                                State = row.Split(',')[6]
                            });
                        }
                    }

                    using (var context = new UsersManagementEntities())
                    {
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy((context.Database.Connection) as SqlConnection))
                        {
                            //Set the database table name.
                            sqlBulkCopy.DestinationTableName = "dbo.UploadData";

                            //[OPTIONAL]: Map the DataTable columns with that of the database table
                            sqlBulkCopy.ColumnMappings.Add("TerminalId", "TerminalId");
                            sqlBulkCopy.ColumnMappings.Add("SerialNo", "SerialNo");
                            sqlBulkCopy.ColumnMappings.Add("MerchantId", "MerchantId");
                            sqlBulkCopy.ColumnMappings.Add("Description", "Description");
                            sqlBulkCopy.ColumnMappings.Add("Manufacturer", "Manufacturer");
                            sqlBulkCopy.ColumnMappings.Add("Model", "Model");
                            sqlBulkCopy.ColumnMappings.Add("State", "State");

                            context.Database.Connection.Open();
                            sqlBulkCopy.WriteToServer(dt);
                            context.Database.Connection.Close();
                        }
                    }
                }
            }

            return View(uploadData);
        }


        public ActionResult UploadFileList()
        {
            return View(db.UploadData.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            UploadData uploadData = db.UploadData.Find(id);
            if (uploadData == null)
            {
                return HttpNotFound();
            }
            return View(uploadData);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UploadData uploadData = db.UploadData.Find(id);
            if (uploadData == null)
            {
                return HttpNotFound();
            }
            return View(uploadData);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UploadData uploadData = db.UploadData.Find(id);
            db.UploadData.Remove(uploadData);
            db.SaveChanges();
            return RedirectToAction("UploadFileList");
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UploadData uploadData = db.UploadData.Find(id);
            if (uploadData == null)
            {
                return HttpNotFound();
            }
            return View(uploadData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TerminalId,SerialNo,MerchantId,Description,Manufacturer,State,Model")] UploadData uploadData)
        {
            if (ModelState.IsValid)
            {
                db.Entry(uploadData).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UploadFileList");
            }
            return View(uploadData);
        }

    }



}