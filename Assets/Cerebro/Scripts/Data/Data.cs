using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
namespace Cerebro
{
	[System.Serializable]
	public abstract class Data<T>
	{
		public List<T> dataList;
		protected string fileName;
		public string versionNumber = "v0.1.0.0";

		public Data()
		{
			this.dataList = new List<T> ();
		}

		public Data(string fileName) : this()
		{
			this.fileName = fileName;
		}

		protected string  GetPath()
		{
			return Application.persistentDataPath +"/"+ fileName+".txt";
		}

		public virtual Data<T> LoadData()
		{
			Debug.Log (GetPath ());
			if (File.Exists (GetPath ())) {
				string json = File.ReadAllText (GetPath ());
				if (string.IsNullOrEmpty (json) || !json.IsValidJSON()) {
					return this;
				}
				JsonUtility.FromJsonOverwrite (json, this);
			}
			return this;
		}

		public virtual void SaveData()
		{
			string json = JsonUtility.ToJson (this);
			File.WriteAllText (GetPath(),json);
		}

		public virtual void Add(T data)
		{
			this.dataList.Add (data);
			this.SaveData ();
		}

		public virtual void Remove(T data)
		{
			this.dataList.Remove (data);
			this.SaveData ();
		}

		public virtual void RemoveAt(int index)
		{
			this.dataList.RemoveAt (index);
			this.SaveData ();
		}

		public int Count()
		{
			return this.dataList.Count;
		}

		public virtual T ElementAt(int index)
		{
			return this.dataList [index];
		}

	}


}