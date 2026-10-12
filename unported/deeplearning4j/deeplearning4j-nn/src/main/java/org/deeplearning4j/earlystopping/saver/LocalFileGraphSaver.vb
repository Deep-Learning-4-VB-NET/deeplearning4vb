Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports org.deeplearning4j.earlystopping
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer

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

Namespace org.deeplearning4j.earlystopping.saver


	Public Class LocalFileGraphSaver
		Implements EarlyStoppingModelSaver(Of ComputationGraph)

		Private Const BEST_GRAPH_BIN As String = "bestGraph.bin"
		Private Const LATEST_GRAPH_BIN As String = "latestGraph.bin"

		Private directory As String
		Private encoding As Charset

		''' <summary>
		'''Constructor that uses default character set for configuration (json) encoding </summary>
		''' <param name="directory"> Directory to save networks </param>
		Public Sub New(ByVal directory As String)
			Me.New(directory, Charset.defaultCharset())
		End Sub

		''' <param name="directory"> Directory to save networks </param>
		''' <param name="encoding"> Character encoding for configuration (json) </param>
		Public Sub New(ByVal directory As String, ByVal encoding As Charset)
			Me.directory = directory
			Me.encoding = encoding

			Dim dir As New File(directory)
			If Not dir.exists() Then
				dir.mkdirs()
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void saveBestModel(org.deeplearning4j.nn.graph.ComputationGraph net, double score) throws java.io.IOException
		Public Overridable Sub saveBestModel(ByVal net As ComputationGraph, ByVal score As Double)
			Dim confOut As String = FilenameUtils.concat(directory, BEST_GRAPH_BIN)
			save(net, confOut)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void saveLatestModel(org.deeplearning4j.nn.graph.ComputationGraph net, double score) throws java.io.IOException
		Public Overridable Sub saveLatestModel(ByVal net As ComputationGraph, ByVal score As Double)
			Dim confOut As String = FilenameUtils.concat(directory, LATEST_GRAPH_BIN)
			save(net, confOut)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void save(org.deeplearning4j.nn.graph.ComputationGraph net, String confOut) throws java.io.IOException
		Private Sub save(ByVal net As ComputationGraph, ByVal confOut As String)
			ModelSerializer.writeModel(net, confOut, True)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.graph.ComputationGraph getBestModel() throws java.io.IOException
		Public Overridable ReadOnly Property BestModel As ComputationGraph
			Get
				Dim confOut As String = FilenameUtils.concat(directory, BEST_GRAPH_BIN)
				Return load(confOut)
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.graph.ComputationGraph getLatestModel() throws java.io.IOException
		Public Overridable ReadOnly Property LatestModel As ComputationGraph
			Get
				Dim confOut As String = FilenameUtils.concat(directory, LATEST_GRAPH_BIN)
				Return load(confOut)
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.deeplearning4j.nn.graph.ComputationGraph load(String confOut) throws java.io.IOException
		Private Function load(ByVal confOut As String) As ComputationGraph
			Dim net As ComputationGraph = ModelSerializer.restoreComputationGraph(confOut)
			Return net
		End Function

		Public Overrides Function ToString() As String
			Return "LocalFileGraphSaver(dir=" & directory & ")"
		End Function
	End Class

End Namespace