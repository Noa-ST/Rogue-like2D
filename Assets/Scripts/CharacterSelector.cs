using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector instance; // Biến tĩnh để lưu singleton instance của CharacterSelector
    public CharacterScriptObject characterData; 

    private void Awake()
    {
        // Kiểm tra nếu chưa có instance nào
        if (instance == null)
        {
            instance = this; // Gán instance hiện tại cho biến tĩnh
            DontDestroyOnLoad(gameObject); // Giữ đối tượng này không bị hủy khi chuyển cảnh
        }
        else
        {
            Debug.LogWarning("EXTRA " + this + " DELETED");
            Destroy(gameObject);
        }
    }

    // Phương thức tĩnh để lấy dữ liệu của nhân vật hiện tại
    public static CharacterScriptObject GetData()
    {
        return instance.characterData; // Trả về characterData của instance
    }

    // Phương thức để chọn một nhân vật mới
    public void SelectCharacter(CharacterScriptObject character)
    {
        characterData = character; // Gán characterData mới
    }

    // Phương thức để hủy singleton và đối tượng
    public void destroySingleTon()
    {
        instance = null; // Đặt instance thành null
        Destroy(gameObject); // Hủy đối tượng
    }
}
