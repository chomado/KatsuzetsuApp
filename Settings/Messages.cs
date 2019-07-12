﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LineKatsuzetsu.Settings
{
    public static class Messages
    {
        public const string WelcomeMessage = "こんにちは。滑舌チェックをしましょう。しゅしゃせんたく　と言ってください。";
        public const string GoodbayMessage = "滑舌チェックご利用ありがとうございました。";
        public const string WrongPronunciationMessage = "すいません。滑舌が悪くて聞き取れませんでした。もう一度試してみましょう。";
        public const string CongratsMessage = "おめでとう！！凄く滑舌よかったですよ。{0} と言っているのがはっきり聞き取れました。またのご利用お待ちしております。";
        public static string GenerateCongratsMessage(string word)
        {
            return string.Format(CongratsMessage, word);
        }
    }
}
