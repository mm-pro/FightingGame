﻿using Demonics.Enum;
using Demonics.Utility;
using System;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public Vector2 _hitboxSize = default;
    public Vector2 _offset = default;
    public Action OnCollision;
    [SerializeField] private bool _hitGround;
    private Color _hitboxColor = Color.red;
    private UnityEngine.LayerMask _hurtboxLayerMask;
    private IHitboxResponder _hitboxResponder;
    [HideInInspector] public bool _hasHit;


    void Start()
	{
        if (_hitboxResponder == null)
        {
            _hitboxResponder = transform.root.GetComponent<IHitboxResponder>();
        }
        _hurtboxLayerMask += LayerProvider.GetLayerMask(LayerMaskEnum.Hurtbox);
        if (_hitGround)
        {
            _hurtboxLayerMask += LayerProvider.GetLayerMask(LayerMaskEnum.Ground);
        }
    }

    public void SetHitboxResponder(Transform hitboxResponder)
    {
        _hitboxResponder = hitboxResponder.GetComponent<IHitboxResponder>();
    }

    void Update()
    {
        Vector2 hitboxPosition = new Vector2(transform.position.x + (_offset.x * transform.root.localScale.x), transform.position.y + (_offset.y * transform.root.localScale.y));
        RaycastHit2D[] hit = Physics2D.BoxCastAll(hitboxPosition, _hitboxSize, 0.0f, Vector2.zero, 0.0f, _hurtboxLayerMask);
        if (hit.Length > 0)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].collider != null)
                {
                    if (_hitboxResponder != null && !hit[i].collider.transform.IsChildOf(transform.root) && !_hasHit)
                    {
                        if (_hitGround && hit[i].normal == Vector2.up)
                        {
                            OnCollision?.Invoke();
                            _hitboxResponder.HitboxCollidedGround(hit[i]);
                        }
                        if (hit[i].collider.transform.TryGetComponent(out Hurtbox hurtbox))
                        {
                            OnCollision?.Invoke();
                            _hitboxResponder.HitboxCollided(hit[i], hurtbox);
                        }
                        _hasHit = true;
                    }
                }
            }
        }
    }

    void OnEnable()
	{
        _hasHit = false;
	}

	void OnDisable()
	{
        _hasHit = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        _hitboxColor.a = 0.6f;
        Vector2 hitboxPosition = new Vector2(transform.position.x + (_offset.x * transform.root.localScale.x), transform.position.y + (_offset.y * transform.root.localScale.y));
        Gizmos.color = _hitboxColor;
        Gizmos.matrix = Matrix4x4.TRS(hitboxPosition, transform.rotation, Vector2.one);

        Gizmos.DrawWireCube(Vector3.zero, new Vector3(_hitboxSize.x, _hitboxSize.y, 1.0f));
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(_hitboxSize.x * 1.01f, _hitboxSize.y * 1.01f, 1.0f));
    }
#endif
}