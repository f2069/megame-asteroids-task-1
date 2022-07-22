using System;
using System.Collections.Generic;
using MegameAsteroids.Core.Dictionares;
using MegameAsteroids.Core.Disposables;
using MegameAsteroids.Core.Extensions;
using MegameAsteroids.Core.Interfaces;
using MegameAsteroids.Models.Reward;
using UnityEngine;

namespace MegameAsteroids.View.Managers {
    public class RewardManager : MonoBehaviour, IRewardManager {
        [SerializeField] private LayerMask playerLayers;
        [SerializeField] private List<TagConstants> playerTags;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private RewardModel _rewardModel;

        private event IRewardManager.OnChange OnChangeEvent;

        private void Awake() {
            _rewardModel = new RewardModel();
        }

        private void OnDestroy()
            => _trash.Dispose();

        public int CurrentScore()
            => _rewardModel.CurrentScore;

        public void ReleaseReward(int rewardPoints) {
            _rewardModel.AddReward(rewardPoints);

            OnChangeEvent?.Invoke();
        }

        public void SubscribeOnDestroyTarget<T>(T target) where T : IDestroyable<T>, IRewarding {
            _trash.Retain(target.SubscribeOnDestroy(ReleaseRewardOnDestroy));
        }

        public IDisposable SubscribeOnChange(IRewardManager.OnChange call) {
            OnChangeEvent += call;

            return new ActionDisposable(() => { OnChangeEvent -= call; });
        }

        private void ReleaseRewardOnDestroy<T>(T target, Transform attacker) where T : IDestroyable<T>, IRewarding {
            if (!attacker.gameObject.IsInLayer(playerLayers)) {
                return;
            }

            if (playerTags.Count != 0 && !playerTags.Exists(enemyTag => attacker.CompareTag(enemyTag.ToString()))) {
                return;
            }

            ReleaseReward(target.RewardComponent.RewardPoints());
        }
    }
}
