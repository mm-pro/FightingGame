using Demonics.Manager;
using SharedGame;
using System;
using UnityEngine;

public class GameSimulationView : MonoBehaviour, IGameView
{
    [SerializeField] private Player _player = default;
    [SerializeField] private TrainingMenu _trainingMenu = default;
    private Player[] playerViews = Array.Empty<Player>();
    private GameManager gameManager => GameManager.Instance;

    private void SetGame(GameSimulation gs)
    {
        var playersGss = GameSimulation._players;
        playerViews = new Player[playersGss.Length];
        playerViews[0] = Instantiate(_player);
        playerViews[1] = Instantiate(_player);
        GameplayManager.Instance.InitializePlayers(playerViews[0].gameObject, playerViews[1].gameObject);
    }

    public void UpdateGameView(IGameRunner runner)
    {
        GameSimulation game = (GameSimulation)runner.Game;
        GameInfo gameInfo = runner.GameInfo;
        var playersGss = GameSimulation._players;
        if (playerViews.Length != playersGss.Length)
        {
            SetGame(game);
        }
        if (GameSimulation.Start)
        {
            GameplayManager.Instance.SetupGame();
            GameSimulation.Start = false;
        }
        for (int i = 0; i < playersGss.Length; ++i)
        {
            playerViews[i].PlayerSimulation.Simulate(playersGss[i], gameInfo.players[i]);
            UpdateEffects(i, playersGss[i].effects);
            UpdateProjectiles(i, playersGss[i].projectiles);
            UpdateAssists(i, playersGss[i].shadow);
            _trainingMenu.SetState(true, playersGss[0].state);
            _trainingMenu.SetState(false, playersGss[1].state);
            _trainingMenu.FramedataValue(true, playersGss[0].resultAttack);
            _trainingMenu.FramedataValue(false, playersGss[1].resultAttack);
        }
    }
    private void UpdateEffects(int index, EffectNetwork[] effects)
    {
        for (int i = 0; i < effects.Length; i++)
        {
            GameObject[] effectObjects = ObjectPoolingManager.Instance.GetPool(index, effects[i].name);
            if (effectObjects.Length > 0)
            {
                for (int j = 0; j < effectObjects.Length; j++)
                {
                    effectObjects[j].SetActive(effects[i].effectGroups[j].active);
                    if (!effectObjects[j].activeSelf)
                    {
                        if (effectObjects[j].TryGetComponent(out PlayerGhost playerGhost))
                        {
                            playerGhost.SetSprite(playerViews[index].PlayerAnimator.GetCurrentSprite());
                        }
                    }
                    else
                    {
                        effectObjects[j].transform.position = new Vector2((int)effects[i].effectGroups[j].position.x, (int)effects[i].effectGroups[j].position.y);
                        effectObjects[j].GetComponent<SpriteRenderer>().flipX = effects[i].effectGroups[j].flip;
                        if (!effectObjects[j].TryGetComponent(out PlayerGhost playerGhost))
                        {
                            effectObjects[j].GetComponent<DemonicsAnimator>().SetAnimation("Idle", effects[i].effectGroups[j].animationFrames);
                        }
                    }
                }
            }
        }
    }
    private void UpdateProjectiles(int index, ProjectileNetwork[] projectiles)
    {
        for (int i = 0; i < projectiles.Length; i++)
        {
            GameObject[] projectileObjects = ObjectPoolingManager.Instance.GetProjectilePool(index, projectiles[i].name);
            if (projectileObjects.Length > 0)
            {
                for (int j = 0; j < projectileObjects.Length; j++)
                {
                    projectileObjects[j].SetActive(projectiles[i].active);
                    if (projectiles[i].active)
                    {
                        projectileObjects[j].transform.position = new Vector2((int)projectiles[i].position.x, (int)projectiles[i].position.y);
                        projectileObjects[j].GetComponent<SpriteRenderer>().flipX = projectiles[i].flip;
                        projectileObjects[j].GetComponent<DemonicsAnimator>().SetAnimation("Idle", projectiles[i].animationFrames);
                        projectileObjects[j].transform.GetChild(0).GetComponent<CollisionVisualizer>().ShowBox(projectiles[i].hitbox);
                    }
                }
            }
        }
    }
    private void UpdateAssists(int index, ShadowNetwork shadow)
    {
        GameObject assistObject = ObjectPoolingManager.Instance.GetAssistPool(index, shadow.projectile.name);
        if (assistObject != null)
        {
            assistObject.SetActive(shadow.projectile.active);
            if (shadow.projectile.active)
            {
                assistObject.transform.position = new Vector2((int)shadow.projectile.position.x, (int)shadow.projectile.position.y);
                assistObject.transform.up = new Vector2((float)shadow.spawnRotation.x * shadow.flip, (float)shadow.spawnRotation.x);
                assistObject.GetComponent<SpriteRenderer>().flipX = shadow.projectile.flip;
                assistObject.GetComponent<DemonicsAnimator>().SetAnimation("Idle", shadow.projectile.animationFrames);
                assistObject.transform.GetChild(0).GetComponent<CollisionVisualizer>().ShowBox(shadow.projectile.hitbox);
            }
        }
    }
    private void Update()
    {
        if (gameManager.IsRunning)
        {
            UpdateGameView(gameManager.Runner);
        }
    }
}
