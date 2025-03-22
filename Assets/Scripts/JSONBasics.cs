// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using Newtonsoft.Json.Linq;
// public class JSONBasics : MonoBehaviour
// {
//    [SerializeField] private TextMeshProUGUI jsonViewText;
//    [SerializeField] private UnityEngine.Object jsonFile;

//     void Start()
//     {
      
//     }


//    public void DisplayJsonData()
//    {
//     jsonViewText.text =jsonFile.ToString();
//    }

//    public void DisplayJsonValue()
//    {
//     JObject json = JObject.Parse(jsonFile.ToString());
//     string firstName = json["user"] ["firstName"].ToString();
//     string age = json["user"] ["age"].ToString();

//     jsonViewText.text = "Name: " + firstName + ", Age: " + age;
//    }
// }

