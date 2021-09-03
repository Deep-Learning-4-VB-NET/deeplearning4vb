Imports System
Imports System.Collections.Generic
Imports System.IO
Imports IOUtils = org.apache.commons.io.IOUtils
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports ResourceType = org.deeplearning4j.common.resources.ResourceType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Downloader = org.nd4j.common.resources.Downloader

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

Namespace org.deeplearning4j.datasets.base


	Public Class IrisUtils

		Private Const IRIS_RELATIVE_URL As String = "datasets/iris.dat"
		Private Const MD5 As String = "1c21400a78061197eac64c6748844216"

		Private Sub New()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static java.util.List<org.nd4j.linalg.dataset.DataSet> loadIris(int from, int to) throws java.io.IOException
		Public Shared Function loadIris(ByVal from As Integer, ByVal [to] As Integer) As IList(Of DataSet)
			Dim rootDir As File = DL4JResources.getDirectory(ResourceType.DATASET, "iris")
			Dim irisData As New File(rootDir, "iris.dat")
			If Not irisData.exists() Then
				Dim url As URL = DL4JResources.getURL(IRIS_RELATIVE_URL)
				Downloader.download("Iris", url, irisData, MD5, 3)
			End If

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") java.util.List<String> lines;
			Dim lines As IList(Of String)
			Using [is] As Stream = New FileStream(irisData, FileMode.Open, FileAccess.Read)
				lines = IOUtils.readLines([is])
			End Using
			Dim list As IList(Of DataSet) = New List(Of DataSet)()
			Dim ret As INDArray = Nd4j.ones(Math.Abs([to] - from), 4)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim outcomes[][] As Double = new Double[lines.Count][3]
			Dim outcomes()() As Double = RectangularArrays.RectangularDoubleArray(lines.Count, 3)
			Dim putCount As Integer = 0

			For i As Integer = from To [to] - 1
				Dim line As String = lines(i)
				Dim split() As String = line.Split(",", True)

'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: addRow(ret, putCount++, split);
				addRow(ret, putCount, split)
					putCount += 1

				Dim outcome As String = split(split.Length - 1)
				Dim rowOutcome(2) As Double
				rowOutcome(Integer.Parse(outcome)) = 1
				outcomes(i) = rowOutcome
			Next i

			Dim i As Integer = 0
			Do While i < ret.rows()
				Dim add As New DataSet(ret.getRow(i, True), Nd4j.create(outcomes(from + i), New Long(){1, 3}))
				list.Add(add)
				i += 1
			Loop
			Return list
		End Function

		Private Shared Sub addRow(ByVal ret As INDArray, ByVal row As Integer, ByVal line() As String)
			Dim vector(3) As Double
			For i As Integer = 0 To 3
				vector(i) = Double.Parse(line(i))
			Next i

			ret.putRow(row, Nd4j.create(vector))
		End Sub
	End Class

End Namespace