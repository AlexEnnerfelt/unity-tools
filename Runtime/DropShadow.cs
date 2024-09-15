
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class DropShadow : MonoBehaviour {
    public Vector2 ShadowOffset = new(0.2f, -0.2f);
    [ColorUsage(showAlpha: true)]
    public Color shadowColor = new(0, 0, 0, 0.4f);

    SpriteRenderer _spriteRenderer;
    GameObject _shadowGameObject;
    SpriteRenderer _shadowSpriteRenderer;

    void Start() {

    }
    public void OnEnable() {
        if (_shadowGameObject != null) {
            _shadowGameObject.SetActive(true);
        }
    }
    public void OnDisable() {
        if (_shadowGameObject != null) {
            _shadowGameObject.SetActive(false);
        }
    }
    public void OnDestroy() {
        if (Application.isPlaying) {
            if (_shadowGameObject != null) {
                Destroy(_shadowGameObject);
            }
        }
    }
    public void OnValidate() {

        if (_spriteRenderer == null) {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        var allShadowChildren = transform.GetAllChildren().ToList().Where(t => t.name == "Shadow");

        if (allShadowChildren.Count() > 1) {
            _shadowGameObject = allShadowChildren.First().gameObject;
            foreach (var item in allShadowChildren) {
                if (item != _shadowGameObject) {
                    //DestroyImmediate(item.gameObject);
                }
            }
        } else if (allShadowChildren.Count() == 1) {
            _shadowGameObject = allShadowChildren.First().gameObject;
        }

        if (_shadowGameObject == null) {
            //create a new gameobject to be used as drop shadow
            _shadowGameObject = new GameObject("Shadow");
            _shadowGameObject.transform.parent = transform;
            //create a new SpriteRenderer for Shadow gameobject
            _shadowSpriteRenderer = _shadowGameObject.AddComponent<SpriteRenderer>();
            _shadowSpriteRenderer.sortingOrder = -1;
        }

        if (_shadowSpriteRenderer == null) {
            _shadowSpriteRenderer = _shadowGameObject.GetComponent<SpriteRenderer>();
        }
        //set the shadow gameobject's sprite to the original sprite
        _shadowSpriteRenderer.sprite = _spriteRenderer.sprite;
        //set the shadow gameobject's material to the shadow material we created
        _shadowSpriteRenderer.material = Addressables.LoadAssetAsync<Material>("Assets/Materials/spriteShadow.mat").WaitForCompletion();

        _shadowSpriteRenderer.color = shadowColor;

        //update the sorting layer of the shadow to always lie behind the sprite
        _shadowSpriteRenderer.sortingLayerName = _spriteRenderer.sortingLayerName;
        _shadowSpriteRenderer.sortingOrder = _spriteRenderer.sortingOrder - 1;
        _shadowGameObject.transform.position = transform.position + (Vector3)ShadowOffset;
        _shadowGameObject.transform.rotation = transform.rotation;
        var scaleFactor = 1 + ShadowOffset.magnitude * 0.1f;
        _shadowGameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, 0);
    }

    void LateUpdate() {
        if (Application.isEditor && !Application.isPlaying) {
            if (_shadowGameObject != null) {
                //update the position and rotation of the sprite's shadow with moving sprite
                _shadowGameObject.transform.position = transform.position + (Vector3)ShadowOffset;
                _shadowGameObject.transform.rotation = transform.rotation;
            }

            var allShadowChildren = transform.GetAllChildren().ToList().Where(t => t.name == "Shadow");

            if (allShadowChildren.Count() > 1) {
                _shadowGameObject = allShadowChildren.First().gameObject;
                foreach (var item in allShadowChildren) {
                    if (item != _shadowGameObject) {
                        DestroyImmediate(item.gameObject);
                    }
                }
            } else if (allShadowChildren.Count() == 1) {
                _shadowGameObject = allShadowChildren.First().gameObject;
            }
        }
    }
}