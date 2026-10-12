Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports org.deeplearning4j.earlystopping
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
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


	Public Class LocalFileModelSaver
		Implements EarlyStoppingModelSaver(Of MultiLayerNetwork)

		Private Const BEST_MODEL_BIN As String = "bestModel.bin"
		Private Const LATEST_MODEL_BIN As String = "latestModel.bin"
		Private directory As String
		Private encoding As Charset

		Public Sub New(ByVal directory As File)
			Me.New(directory.getAbsolutePath())
		End Sub

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
'ORIGINAL LINE: @Override public void saveBestModel(org.deeplearning4j.nn.multilayer.MultiLayerNetwork net, double score) throws java.io.IOException
		Public Overridable Sub saveBestModel(ByVal net As MultiLayerNetwork, ByVal score As Double)
			Dim confOut As String = FilenameUtils.concat(directory, BEST_MODEL_BIN)
			save(net, confOut)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void saveLatestModel(org.deeplearning4j.nn.multilayer.MultiLayerNetwork net, double score) throws java.io.IOException
		Public Overridable Sub saveLatestModel(ByVal net As MultiLayerNetwork, ByVal score As Double)
			Dim confOut As String = FilenameUtils.concat(directory, LATEST_MODEL_BIN)
			save(net, confOut)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.multilayer.MultiLayerNetwork getBestModel() throws java.io.IOException
		Public Overridable ReadOnly Property BestModel As MultiLayerNetwork
			Get
				Dim confOut As String = FilenameUtils.concat(directory, BEST_MODEL_BIN)
				Return load(confOut)
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.multilayer.MultiLayerNetwork getLatestModel() throws java.io.IOException
		Public Overridable ReadOnly Property LatestModel As MultiLayerNetwork
			Get
				Dim confOut As String = FilenameUtils.concat(directory, LATEST_MODEL_BIN)
				Return load(confOut)
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void save(org.deeplearning4j.nn.multilayer.MultiLayerNetwork net, String modelName) throws java.io.IOException
		Private Sub save(ByVal net As MultiLayerNetwork, ByVal modelName As String)
			ModelSerializer.writeModel(net, modelName, True)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.deeplearning4j.nn.multilayer.MultiLayerNetwork load(String modelName) throws java.io.IOException
		Private Function load(ByVal modelName As String) As MultiLayerNetwork
			Dim net As MultiLayerNetwork = ModelSerializer.restoreMultiLayerNetwork(modelName)
			Return net
		End Function

		Public Overrides Function ToString() As String
			Return "LocalFileModelSaver(dir=" & directory & ")"
		End Function
	End Class

End Namespace