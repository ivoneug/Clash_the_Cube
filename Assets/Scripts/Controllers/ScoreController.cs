using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Variables;
using Databox;

namespace ClashTheCube 
{
    public class ScoreController : MonoBehaviour
    {
        [SerializeField] private DataboxObject databox;
        [SerializeField] private IntReference nextCubeNumber;
        [SerializeField] private UILabel label;

        private int score;
        private int destScore;

        public void LoadSavedState()
        {
            LoadScore();
            label.text = score.ToString();
        }

        private void Update()
        {
            if (score < destScore)
            {
                score += 10;
                if (score > destScore)
                {
                    score = destScore;
                }

                label.text = score.ToString();
            }
        }

        private void LoadScore()
        {
            var table = DataBaseController.Data_Table;
            var entry = DataBaseController.Stats_Entry;
            var scoreField = DataBaseController.Stats_ScoreField;

            if (!databox.EntryExists(table, entry))
            {
                score = 0;
                databox.AddData(table, entry, scoreField, new IntType(score));
            }
            else
            {
                score = databox.GetData<IntType>(table, entry, scoreField).Value;
            }

            destScore = score;
        }

        public void UpdateScore()
        {
            UpdateScoreInternal(nextCubeNumber);
        }

        private void UpdateScoreInternal(int value)
        {
            var table = DataBaseController.Data_Table;
            var entry = DataBaseController.Stats_Entry;
            var scoreField = DataBaseController.Stats_ScoreField;

            destScore += nextCubeNumber;

            databox.SetData<IntType>(table, entry, scoreField, new IntType(destScore));
        }

        public void ScoreClear()
        {
            var table = DataBaseController.Data_Table;
            var entry = DataBaseController.Stats_Entry;
            var scoreField = DataBaseController.Stats_ScoreField;

            databox.SetData<IntType>(table, entry, scoreField, new IntType(0));
        }
    }
}
