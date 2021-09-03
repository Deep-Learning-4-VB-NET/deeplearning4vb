Imports System
Imports System.Collections.Generic
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports DataAnalysis = org.datavec.api.transform.analysis.DataAnalysis
Imports NumericalColumnAnalysis = org.datavec.api.transform.analysis.columns.NumericalColumnAnalysis
Imports Schema = org.datavec.api.transform.schema.Schema
Imports RecordConverter = org.datavec.api.util.ndarray.RecordConverter
Imports Writable = org.datavec.api.writable.Writable
Imports AnalyzeLocal = org.datavec.local.transforms.AnalyzeLocal
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
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

Namespace org.datavec.local.transforms.analysis


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) public class TestAnalyzeLocal
	Public Class TestAnalyzeLocal
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAnalysisBasic() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAnalysisBasic()

			Dim rr As RecordReader = New CSVRecordReader()
			rr.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("iris.txt")).File))

			Dim s As Schema = (New Schema.Builder()).addColumnsDouble("0", "1", "2", "3").addColumnInteger("label").build()

			Dim da As DataAnalysis = AnalyzeLocal.analyze(s, rr)

			Console.WriteLine(da)

			'Compare:
			Dim list As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			rr.reset()
			Do While rr.hasNext()
				list.Add(rr.next())
			Loop

			Dim arr As INDArray = RecordConverter.toMatrix(DataType.DOUBLE, list)
			Dim mean As INDArray = arr.mean(0)
			Dim std As INDArray = arr.std(0)

			For i As Integer = 0 To 4
				Dim m As Double = CType(da.getColumnAnalysis().get(i), NumericalColumnAnalysis).getMean()
				Dim stddev As Double = CType(da.getColumnAnalysis().get(i), NumericalColumnAnalysis).getSampleStdev()
				assertEquals(mean.getDouble(i), m, 1e-3)
				assertEquals(std.getDouble(i), stddev, 1e-3)
			Next i

		End Sub

	End Class

End Namespace