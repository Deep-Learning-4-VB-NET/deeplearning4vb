Imports System
Imports System.Collections.Generic
Imports TDigest = com.tdunning.math.stats.TDigest
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports DataAnalysis = org.datavec.api.transform.analysis.DataAnalysis
Imports SequenceDataAnalysis = org.datavec.api.transform.analysis.SequenceDataAnalysis
Imports ColumnAnalysis = org.datavec.api.transform.analysis.columns.ColumnAnalysis
Imports IntegerAnalysis = org.datavec.api.transform.analysis.columns.IntegerAnalysis
Imports StringAnalysis = org.datavec.api.transform.analysis.columns.StringAnalysis
Imports TimeAnalysis = org.datavec.api.transform.analysis.columns.TimeAnalysis
Imports SequenceLengthAnalysis = org.datavec.api.transform.analysis.sequence.SequenceLengthAnalysis
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports DateTimeZone = org.joda.time.DateTimeZone
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.datavec.api.transform.ui


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) @Tag(TagNames.UI) public class TestUI extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestUI
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUI(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUI(ByVal testDir As Path)
			Dim schema As Schema = (New Schema.Builder()).addColumnString("StringColumn").addColumnInteger("IntColumn").addColumnInteger("IntColumn2").addColumnInteger("IntColumn3").addColumnTime("TimeColumn", DateTimeZone.UTC).build()

			Dim list As IList(Of ColumnAnalysis) = New List(Of ColumnAnalysis)()
			list.Add((New StringAnalysis.Builder()).countTotal(10).maxLength(7).countTotal(999999999L).minLength(99999999).maxLength(99999999).meanLength(9999999999.0).sampleStdevLength(99999999.0).sampleVarianceLength(0.99999999999).histogramBuckets(New Double() {0, 1, 2, 3, 4, 5}).histogramBucketCounts(New Long() {50, 30, 10, 12, 3}).build())

			list.Add((New IntegerAnalysis.Builder()).countTotal(10).countMaxValue(1).countMinValue(4).min(0).max(30).countTotal(999999999).countMaxValue(99999999).countMinValue(999999999).min(-999999999).max(9999999).min(99999999).max(99999999).mean(9999999999.0).sampleStdev(99999999.0).sampleVariance(0.99999999999).histogramBuckets(New Double() {-3, -2, -1, 0, 1, 2, 3}).histogramBucketCounts(New Long() { 100_000_000, 20_000_000, 30_000_000, 40_000_000, 50_000_000, 60_000_000}).build())

			list.Add((New IntegerAnalysis.Builder()).countTotal(10).countMaxValue(1).countMinValue(4).min(0).max(30).histogramBuckets(New Double() {-3, -2, -1, 0, 1, 2, 3}).histogramBucketCounts(New Long() {15, 20, 35, 40, 55, 60}).build())

			Dim t As TDigest = TDigest.createDigest(100)
			For i As Integer = 0 To 99
				t.add(i)
			Next i
			list.Add((New IntegerAnalysis.Builder()).countTotal(10).countMaxValue(1).countMinValue(4).min(0).max(30).histogramBuckets(New Double() {-3, -2, -1, 0, 1, 2, 3}).histogramBucketCounts(New Long() {10, 2, 3, 4, 5, 6}).digest(t).build())

			list.Add((New TimeAnalysis.Builder()).min(1451606400000L).max(1451606400000L + 60000L).build())


			Dim da As New DataAnalysis(schema, list)

			Dim fDir As File = testDir.toFile()
			Dim tempDir As String = fDir.getAbsolutePath()
			Dim outPath As String = FilenameUtils.concat(tempDir, "datavec_transform_UITest.html")
			Console.WriteLine(outPath)
			Dim f As New File(outPath)
			f.deleteOnExit()
			HtmlAnalysis.createHtmlAnalysisFile(da, f)


			'Test JSON:
			Dim json As String = da.toJson()
			Dim fromJson As DataAnalysis = DataAnalysis.fromJson(json)
			assertEquals(da, fromJson)



			'Test sequence analysis:
			Dim sla As SequenceLengthAnalysis = SequenceLengthAnalysis.builder().totalNumSequences(100).minSeqLength(1).maxSeqLength(50).countZeroLength(0).countOneLength(10).meanLength(20.0).histogramBuckets(New Double(){0.0, 1.0, 2.0, 3.0, 4.0, 5.0}).histogramBucketCounts(New Long(){1, 2, 3, 4, 5}).build()
			Dim sda As New SequenceDataAnalysis(da.getSchema(), da.getColumnAnalysis(), sla)


			'HTML:
			outPath = FilenameUtils.concat(tempDir, "datavec_transform_UITest_seq.html")
			Console.WriteLine(outPath)
			f = New File(outPath)
			f.deleteOnExit()
			HtmlAnalysis.createHtmlAnalysisFile(sda, f)


			'JSON
			json = sda.toJson()
			Dim sFromJson As SequenceDataAnalysis = SequenceDataAnalysis.fromJson(json)

			Dim toStr1 As String = sda.ToString()
			Dim toStr2 As String = sFromJson.ToString()
			assertEquals(toStr1, toStr2)

			assertEquals(sda, sFromJson)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testSequencePlot() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSequencePlot()

			Dim schema As Schema = (New SequenceSchema.Builder()).addColumnDouble("sinx").addColumnCategorical("cat", "s0", "s1", "s2").addColumnString("stringcol").build()

			Dim nSteps As Integer = 100
			Dim sequence As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(nSteps)
			For i As Integer = 0 To nSteps - 1
				Dim c As String = "s" & i Mod 3
				sequence.Add(New List(Of Writable) From {Of Writable})
			Next i

			Dim tempDir As String = System.getProperty("java.io.tmpdir")
			Dim outPath As String = FilenameUtils.concat(tempDir, "datavec_seqplot_test.html")
			'        System.out.println(outPath);
			Dim f As New File(outPath)
			f.deleteOnExit()
			HtmlSequencePlotting.createHtmlSequencePlotFile("Title!", schema, sequence, f)


		End Sub
	End Class

End Namespace