using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class ImportExportJSON<T> : MonoBehaviour where T : class
{
    public bool ImportAtStart = true;
    public bool WaitBeforeImportAtStart = true;

    [InspectorButton("Import", ButtonWidth = 150)]
    public bool ImportBtn;

    [InspectorButton("Export", ButtonWidth = 150)]
    public bool ExportBtn;

    protected bool _isDoingImportExport = false;

    protected Coroutine m_myCoroutineRef;

    protected void Start()
    {
        if (!ImportAtStart) return;
        _isDoingImportExport = true;
        if (WaitBeforeImportAtStart)
        {
            if (m_myCoroutineRef != null)
            {
                StopCoroutine(m_myCoroutineRef);
                m_myCoroutineRef = null;
            }
            m_myCoroutineRef = StartCoroutine(WaitBeforeFirstImport());
        }
        else
        {
            DoImport();
        }

    }
    protected IEnumerator WaitBeforeFirstImport()
    {
        yield return null;
        yield return null;

        DoImport();
    }



    protected void DoImport()
    {
        _isDoingImportExport = true;
        bool success = TryImport();

        if (!success)
        {
            ActionWhenImportFail();
        }
        _isDoingImportExport = false;
    }

    protected virtual void ActionWhenImportFail()
    {
        InformationText.Show("Missing data. Contact an admin.");
    }


    protected bool TryImport()
    {
        string path = GetImportDataPath();
        if (!File.Exists(path))
        {
            Debug.Log("Missing JSON file !");
            return false;
        }

        string content = File.ReadAllText(path, Encoding.UTF8);

        T[] arrayOfData = JsonSerialiserService.DeserialyseArray<T>(content);
        if (arrayOfData == null || arrayOfData.Length == 0)
        {
            Debug.Log("Wrong or empty JSON file !");
            return false;
        }
        DoActionWithObtainedData(arrayOfData.ToList());

        return true;
    }



    protected bool TryExport()
    {
        T[] arrayOfData = GetArrayOfData();
        if (arrayOfData == null) return false;
        string content = JsonSerialiserService.SerialyseArray(arrayOfData);

        string pathToSaveTo = GetExportDataPath();
        //Debug.Log(pathToSaveTo);

        File.WriteAllText(pathToSaveTo, content, Encoding.UTF8);
        return true;
    }

    protected void DestroyFile()
    {
        string importPath = GetImportDataPath();
        if (File.Exists(importPath))
        {
            File.Delete(importPath);
        }
        string exportPath = GetExportDataPath();
        if (File.Exists(exportPath))
        {
            File.Delete(exportPath);
        }

    }
    internal string GetImportDataPath()
    {
        return Application.persistentDataPath + GetImportPathEnd();
    }
    internal string GetExportDataPath()
    {
        return Application.persistentDataPath + GetExportPathEnd();
    }

    #region CALLED_BY_BUTTONS OR CLASSES
    public void Import()
    {
        if (_isDoingImportExport) return;
        DoImport();
    }

    public void Export()
    {
        if (_isDoingImportExport) return;
        _isDoingImportExport = true;
        bool success = TryExport();

        if (!success)
        {
            InformationText.Show("Export failed. Contact an admin.");
        }

        _isDoingImportExport = false;
    }
    #endregion



    protected abstract string GetImportPathEnd();
    protected abstract string GetExportPathEnd();

    protected abstract void DoActionWithObtainedData(List<T> dataList);


    protected abstract T[] GetArrayOfData();
}

