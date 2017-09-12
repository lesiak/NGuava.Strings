﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NGuava.Base;

namespace NGuava.Tests.Base
{
    [TestClass]
    public class SplitterTest
    {
        private static readonly Splitter COMMA_SPLITTER = Splitter.On(',');

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestSplitNullString()
        {
            COMMA_SPLITTER.split(null);
        }

        [TestMethod]
        public void TestCharacterSimpleSplit()
        {
            const string simple = "a,b,c";
            var letters = COMMA_SPLITTER.split(simple);
            letters.Should().BeEquivalentTo(new List<string> {"a", "b", "c"},
                options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void TestCharacterSimpleSplitToList()
        {
            const string simple = "a,b,c";
            var letters = COMMA_SPLITTER.SplitToList(simple);
            letters.Should().BeEquivalentTo(new List<string> { "a", "b", "c" },
                options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void TestToString()
        {
            Splitter.On(',').split("").ToString().Should().Be("[]");
            Splitter.On(',').split("a,b,c").ToString().Should().Be("[a, b, c]");
            Splitter.On(", ").split("yam, bam, jam, ham").ToString().Should().Be("[yam, bam, jam, ham]"); 
        }

        [TestMethod]
        public void TestSplitterIsUsableAfterToString()
        {
            var letters = Splitter.On(',').split("a,b,c");
            letters.ToString().Should().Be("[a, b, c]");
            letters.Should().BeEquivalentTo(new List<string> { "a", "b", "c" },
                options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void TestCharacterSimpleSplitWithNoDelimiter()
        {
            const string simple = "a,b,c";
            var letters = Splitter.On('.').split(simple);
            letters.Should().BeEquivalentTo(new List<string> { "a,b,c" },
                options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void TestCharacterSplitWithDoubleDelimiter()
        {
            const string doubled = "a,,b,c";
            var letters = COMMA_SPLITTER.split(doubled);
            letters.Should().BeEquivalentTo(new List<string> { "a", "", "b", "c" },
                options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void TestCharacterSplitWithDoubleDelimiterAndSpace()
        {
            const string doubled = "a,, b,c";
            var letters = COMMA_SPLITTER.split(doubled);
            letters.Should().BeEquivalentTo(new List<string> { "a", "", " b", "c" },
                options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void TestCharacterSplitWithTrailingDelimiter()
        {
            const string trailing = "a,b,c,";
            var letters = COMMA_SPLITTER.split(trailing);
            letters.Should().BeEquivalentTo(new List<string> { "a", "b", "c", "" },
                options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void TestCharacterSplitWithLeadingDelimiter()
        {
            const string leading = ",a,b,c";
            var letters = COMMA_SPLITTER.split(leading);
            letters.Should().BeEquivalentTo(new List<string> { "", "a", "b", "c" },
                options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void TestCharacterSplitWithMulitpleLetters()
        {
            var testCharacteringMotto = Splitter.On('-').split(
                "Testing-rocks-Debugging-sucks");
            testCharacteringMotto.Should()
                .BeEquivalentTo(new List<string> { "Testing", "rocks", "Debugging", "sucks" },
                    options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void TestCharacterSplitWithMatcherDelimiter()
        {
            var testCharacteringMotto = Splitter
                .On(CharMatcher.Whitespace)
                .split("Testing\nrocks\tDebugging sucks");
            testCharacteringMotto.Should()
                .BeEquivalentTo(new List<string> { "Testing", "rocks", "Debugging", "sucks" },
                    options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void TestCharacterSplitWithDoubleDelimiterOmitEmptyStrings()
        {
            const string doubled = "a..b.c";
            var letters = Splitter.On('.')
                .OmitEmptyStrings().split(doubled);
            letters.Should().BeEquivalentTo(new List<string> { "a", "b", "c" },
                options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void TestCharacterSplitEmptyToken()
        {
            const string emptyToken = "a. .c";
            var letters = Splitter.On('.').TrimResults()
                .split(emptyToken);
            letters.Should().BeEquivalentTo(new List<string> { "a", "", "c" },
                options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void TestCharacterSplitEmptyTokenOmitEmptyStrings()
        {
            const string emptyToken = "a. .c";
            var letters = Splitter.On('.')
                .OmitEmptyStrings().TrimResults().split(emptyToken);
            letters.Should().BeEquivalentTo(new List<string> { "a", "c" },
                options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void TestCharacterSplitOnEmptyString()
        {
            var nothing = Splitter.On('.').split("");
            nothing.Should().BeEquivalentTo(new List<string> { "" },
                options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void TestCharacterSplitOnEmptyStringOmitEmptyStrings()
        {
            Splitter.On('.').OmitEmptyStrings().split("").Should().BeEmpty();
        }

        [TestMethod]
        public void TestCharacterSplitOnOnlyDelimiter()
        {
           var blankblank = Splitter.On('.').split(".");
            blankblank.Should().BeEquivalentTo(new List<string> { "", "" },
                options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void TestCharacterSplitOnOnlyDelimitersOmitEmptyStrings()
        {
            var empty = Splitter.On('.').OmitEmptyStrings().split("...");
            empty.Should().BeEmpty();
        }

        [TestMethod]
        public void TestCharacterSplitWithTrim()
        {
            const string jacksons = "arfo(Marlon)aorf, (Michael)orfa, afro(Jackie)orfa, "
                                    + "ofar(Jemaine), aff(Tito)";
            var family = COMMA_SPLITTER
                .TrimResults(CharMatcher.anyOf("afro").Or(CharMatcher.Whitespace))
                .split(jacksons);
            family.Should().BeEquivalentTo(new List<string>
                {
                    "(Marlon)",
                    "(Michael)",
                    "(Jackie)",
                    "(Jemaine)",
                    "(Tito)"
                },
                options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void TestStringSimpleSplit()
        {
            const string simple = "a,b,c";
            var letters = Splitter.On(',').split(simple);
            letters.Should().BeEquivalentTo(new List<string> { "a", "b", "c" },
                options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void TestStringSimpleSplitWithNoDelimiter()
        {
            const string simple = "a,b,c";
            var letters = Splitter.On('.').split(simple);
            letters.Should().BeEquivalentTo(new List<string> { "a,b,c" },
                options => options.WithStrictOrdering());
        }

        [TestMethod]
        public void TestStringSplitWithDoubleDelimiter()
        {
            const string doubled = "a,,b,c";
            var letters = Splitter.On(',').split(doubled);
            letters.Should().BeEquivalentTo(new List<string> { "a", "", "b", "c" },
                options => options.WithStrictOrdering());
           
        }

    }
}