#!/bin/perl

# File names
$keyword_file     = "KeywordList.txt";
$keywords_outfile = "Keywords.cs";
$tokens_outfile   = "Tokens.cs";
$unittests_outfile = "LexerTests.cs";
$ATGTokensSection = "ATGTokensSection.gen";

#read infile
print "\n";
print "Reading keyword definition from '$keyword_file'.\n";
open(DAT, $keyword_file) || die("Could not open file '$keyword_file': $!");
@raw_data=<DAT>;
close(DAT);
print "done.\n";

#analyse infile
print "starting analysation ... this could take a few minutes.\n";

foreach (@raw_data) {
	if ($_=~/^\s*\$(\w+)\s*=\s*(\S+)\s*$/) {
		#properties form: $PROPERTY = "VALUE"
		$properties{$1} = $2;
	} elsif  ($_=~/^\s*(\w+)\s*=\s*(\S+)\s*$/) {
		#special characters form: name = "VALUE"
		$specialCharLookup{$2} = $1;
		$special_chars[$#special_chars + 1] = $1;
		$special_values[$#special_values + 1] = $2;
	} elsif  ($_=~/^\s*\"(\S+)\s*\"\s*$/) {
		#special keywords form: "VALUE"
		$keywords[$#keywords + 1] = $1;
	} elsif ($_=~/^\s*(\w+)\s*\((.*)\)\s*$/) {
		$sets[$#sets + 1] = $1;
		#Split set values (comma separated list)
		$_ = $2;
		@vals = split/\s*,\s*/;

		push @$setValues, [@vals];
	} elsif  ($_=~/^\s*(\w+)\s*$/) {
		#special terminal classes form: name
		$terminals[$#terminals + 1] = $1
	} elsif  ($_=~/^\s*(#.*)?$/) {
		#ignore empty line
	} else {
		print "unknown line: $_";
	}
}


for ($i=0; $i <= $#keywords; $i++) {
	$upperKeywords[$i] = uc $keywords[$i];
}
sort (ascend @upperKeywords);


sort (ascend @keywords);
print "done.\n";

#write output
print "writing output files.\nIf your computer doesn�t respond, then press \"Ctrl-Alt-Delete\"\n";
print "\n";
&write_keywordfile;
print "\n";
&write_tokensfile;
print "\n";
&write_atgtokensfile;
print "\n";
&write_unittests;
print "finished.\n";

sub write_keywordfile
{
	print "  ->Generating Keywords class to file '$keywords_outfile'\n";
	open(DAT,">$keywords_outfile") || die("Cannot Open File");
	print DAT "// this file was autogenerated by a tool.\n";
	print DAT "using System;\n";
	print DAT "\n";
	print DAT "namespace " . $properties{'Namespace'} . "\n";
	print DAT "{\n";
	print DAT "\tpublic class Keywords\n";
	print DAT "\t{\n";
	print DAT "\t\tstatic readonly string[] keywordList = {\n";
	if ($properties{'UpperCaseKeywords'} eq "True") {
		for ($i=0; $i <= $#upperKeywords; $i++) {
			print DAT "\t\t\t\"$upperKeywords[$i]\"";
			if ($i + 1 <= $#upperKeywords) {
				print DAT ",";
			}
			print DAT "\n";
		}
	} else {
		for ($i=0; $i <= $#keywords; $i++) {
			print DAT "\t\t\t\"$keywords[$i]\"";
			if ($i + 1 <= $#keywords) {
				print DAT ",";
			}
			print DAT "\n";
		}
	}
	
	print DAT "\t\t};\n";
	print DAT "\t\t\n";
	if ($properties{'UpperCaseKeywords'} eq "True") {
		print DAT "\t\tstatic LookupTable keywords = new LookupTable(false);\n";
	} else {
		print DAT "\t\tstatic LookupTable keywords = new LookupTable(true);\n";	
	}
	
	print DAT "\t\t\n";
	print DAT "\t\tstatic Keywords()\n";
	print DAT "\t\t{\n";
	print DAT "\t\t\tfor (int i = 0; i < keywordList.Length; ++i) {\n";
	print DAT "\t\t\t\tkeywords[keywordList[i]] = i + Tokens." .  ucfirst $keywords[0] . ";\n";
	print DAT "\t\t\t}\n";
	print DAT "\t\t}\n";
	print DAT "\t\t\n";
	print DAT "\t\tpublic static int GetToken(string keyword)\n";
	print DAT "\t\t{\n";
	print DAT "\t\t\treturn keywords[keyword];\n";
	print DAT "\t\t}\n";
	print DAT "\t}\n";
	print DAT "}\n";
	close(DAT);
	print "  ->done.\n";
}

sub write_token {
	$formattedString = sprintf("%-20s", ucfirst $tokenName);
	if ($tokenName eq "GetType") {
		print DAT "\t\tnew public const int $formattedString = $tokenValue;\n";
	} else {
		print DAT "\t\tpublic const int $formattedString = $tokenValue;\n";
	}
	$tokenValue++;
	
}

sub print_list {
	local ($j, $k, $max, $index);

	$index = $_[0];
	$max   = $#{$setValues->[$index]};
	
	for ($j=0; $j <= $max; $j++) {
		$_ = $setValues->[$index][$j];
		if (/\"(\w+)\"/) { # Keywords
			print DAT ucfirst $1;
		} elsif (/\"(\W+)\"/) { # special chars
			print DAT $specialCharLookup{$_};
		} elsif (/@(\w+)/) { # @otherList
			for ($k=0; $k <= $#sets; $k++) {
				if ($sets[$k] eq $1) {
					print_list($k);
				}
			}
		} else {
			print DAT $_;
		}
		
		if ($j + 1 <= $max) {
			print DAT ", ";				
		}
	}
}


sub write_tokensfile {
	print "  ->Generating Tokens class to file '$tokens_outfile'\n";
	open(DAT,">$tokens_outfile") || die("Cannot Open File");
	print DAT "// this file was autogenerated by a tool.\n";
	print DAT "using System;\n";
	print DAT "using System.Collections;\n";
	print DAT "\n";
	print DAT "namespace " . $properties{'Namespace'} . "\n";
	print DAT "{\n";
	print DAT "\tpublic static class Tokens\n";
	print DAT "\t{\n";
	$tokenValue = 0;
	
	print DAT "\t\t// ----- terminal classes -----\n";
	foreach (@terminals) {
		$tokenName = $_;
		write_token();
	}
	print DAT "\n";
	print DAT "\t\t// ----- special character -----\n";
	foreach (@special_chars) {
		$tokenName = $_;
		write_token();
	}
	print DAT "\n";
	print DAT "\t\t// ----- keywords -----\n";
	foreach (@keywords) {
		$tokenName = $_;
		write_token();
	}
	print DAT "\n";
	
	print DAT "\t\tpublic const int MaxToken = " . $tokenValue . ";\n";
	
	#write sets.
	if ($#sets > 0) {
		print DAT "\t\tstatic BitArray NewSet(params int[] values)\n";
		print DAT "\t\t{\n";
		print DAT "\t\t\tBitArray bitArray = new BitArray(MaxToken);\n";
		print DAT "\t\t\tforeach (int val in values) {\n";
		print DAT "\t\t\tbitArray[val] = true;\n";
		print DAT "\t\t\t}\n";
		print DAT "\t\t\treturn bitArray;\n";
		print DAT "\t\t}\n";
		for ($i=0; $i <= $#sets; $i++) {
			print DAT "\t\tpublic static BitArray ". $sets[$i] . " = NewSet(";
			print_list($i);
			print DAT ");\n";
		}
		print DAT "\n";		
	}
	
	#write token number --> string function.
	print DAT "\t\tstatic string[] tokenList = new string[] {\n";
	print DAT "\t\t\t// ----- terminal classes -----\n";
	foreach (@terminals) {
		print DAT "\t\t\t\"<$_>\",\n";
	}
	print DAT "\t\t\t// ----- special character -----\n";
	foreach (@special_values) {
		print DAT "\t\t\t$_,\n";
	}
	print DAT "\t\t\t// ----- keywords -----\n";
	foreach (@keywords) {
		print DAT "\t\t\t\"$_\",\n";
	}
	print DAT "\t\t};\n";
	
	
	print DAT "\t\tpublic static string GetTokenString(int token)\n";
	print DAT "\t\t{\n";
	print DAT "\t\t\tif (token >= 0 && token < tokenList.Length) {\n";
	print DAT "\t\t\t\treturn tokenList[token];\n";
	print DAT "\t\t\t}\n";
	print DAT "\t\t\tthrow new System.NotSupportedException(\"Unknown token:\" + token);\n";
	print DAT "\t\t}\n";
	print DAT "\t}\n";
	
	
	
	print DAT "}\n";
	close(DAT);
	print "  ->done.\n";
}

sub write_unittests {
	open(DAT,">$unittests_outfile") || die("Cannot Open File");
	print DAT "using System;\n";
	print DAT "using System.IO;\n";
	print DAT "using NUnit.Framework;\n";
	print DAT "using ICSharpCode.NRefactory.Parser;\n";
	print DAT "using ICSharpCode.NRefactory.PrettyPrinter;\n";
	
	print DAT "\n";
	print DAT "namespace ICSharpCode.NRefactory.Tests.Lexer\n";
	print DAT "{\n";
	print DAT "\t[TestFixture]\n";
	print DAT "\tpublic sealed class LexerTests\n";
	print DAT "\t{\n";
	print DAT "\t\tILexer GenerateLexer(StringReader sr)\n";
	print DAT "\t\t{\n";
	print DAT "\t\t\treturn ParserFactory.CreateLexer(SupportedLanguages.CSharp, sr);\n";
	print DAT "\t\t}\n\n";

	for ($i=0; $i < $#special_values; $i++) {
	
		print DAT "\t\t[Test]\n";
		print DAT "\t\tpublic void Test" . $special_chars[$i] ."()\n";
		print DAT "\t\t{\n";
		print DAT "\t\t\tILexer lexer = GenerateLexer(new StringReader(" . $special_values[$i] . "));\n";
		print DAT "\t\t\tAssert.AreEqual(Tokens." . $special_chars[$i] . ", lexer.NextToken().kind);\n";
		print DAT "\t\t}\n\n";
		
	}

	foreach (@keywords) {
		print DAT "\t\t[Test()]\n";
		print DAT "\t\tpublic void Test" . ucfirst $_ ."()\n";
		print DAT "\t\t{\n";
		print DAT "\t\t\tILexer lexer = GenerateLexer(new StringReader(\"" . $_ . "\"));\n";
		print DAT "\t\t\tAssert.AreEqual(Tokens." . ucfirst $_ . ", lexer.NextToken().kind);\n";
		print DAT "\t\t}\n";
	}
	
	print DAT "\t}\n";
	print DAT "}\n";

	
	close(DAT);
}

sub write_atgtokensfile {
	print "  ->Generating ATG TOKENS section and writing it to file '$ATGTokensSection'\n";
	open(DAT,">$ATGTokensSection") || die("Cannot Open File");
	print DAT "/* START AUTOGENERATED TOKENS SECTION */\n";
	print DAT "TOKENS\n";

	print DAT "\t/* ----- terminal classes ----- */\n";
	print DAT "\t/* EOF is 0 */\n";
	foreach $term (@terminals) {
		if ($term eq "EOF") {
		} elsif ($term eq "Identifier") {
			print DAT "\tident\n";
		} else {
			print DAT "\t$term\n";
		}
			
	}
	
	print DAT "\n";
	print DAT "\t/* ----- special character ----- */\n";
	foreach (@special_values) {
		print DAT "\t$_\n";
	}
	print DAT "\n";
	print DAT "\t/* ----- keywords ----- */\n";
	foreach (@keywords) {
		print DAT "\t\"$_\"\n";
	}

	print DAT "/* END AUTOGENERATED TOKENS SECTION */\n";
	close(DAT);
	print "  ->done.\n";
}

