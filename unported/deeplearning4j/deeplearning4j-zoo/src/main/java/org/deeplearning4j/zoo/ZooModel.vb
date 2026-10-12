Imports System
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports ResourceType = org.deeplearning4j.common.resources.ResourceType
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
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

Namespace org.deeplearning4j.zoo


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class ZooModel<T> implements InstantiableModel
	Public MustInherit Class ZooModel(Of T)
		Implements InstantiableModel

		Public MustOverride Function pretrainedChecksum(ByVal pretrainedType As PretrainedType) As Long Implements InstantiableModel.pretrainedChecksum
		Public MustOverride Function pretrainedUrl(ByVal pretrainedType As PretrainedType) As String Implements InstantiableModel.pretrainedUrl
		Public MustOverride Function modelType() As Type Implements InstantiableModel.modelType
		Public MustOverride Function metaData() As ModelMetaData Implements InstantiableModel.metaData
		Public MustOverride Function init() As M Implements InstantiableModel.init
		Public MustOverride WriteOnly Property InputShape As Integer()()

		Public Overridable Function pretrainedAvailable(ByVal pretrainedType As PretrainedType) As Boolean
			Return pretrainedUrl(pretrainedType) IsNot Nothing
		End Function

		''' <summary>
		''' By default, will return a pretrained ImageNet if available.
		''' 
		''' @return </summary>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.nn.api.Model initPretrained() throws java.io.IOException
		Public Overridable Function initPretrained() As Model
			Return initPretrained(PretrainedType.IMAGENET)
		End Function

		''' <summary>
		''' Returns a pretrained model for the given dataset, if available.
		''' </summary>
		''' <param name="pretrainedType">
		''' @return </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public <M extends org.deeplearning4j.nn.api.Model> M initPretrained(PretrainedType pretrainedType) throws java.io.IOException
		Public Overridable Function initPretrained(Of M As Model)(ByVal pretrainedType As PretrainedType) As M
			Dim remoteUrl As String = pretrainedUrl(pretrainedType)
			If remoteUrl Is Nothing Then
				Throw New System.NotSupportedException("Pretrained " & pretrainedType & " weights are not available for this model.")
			End If

			Dim localFilename As String = Path.GetFileName(remoteUrl)

			Dim rootCacheDir As File = DL4JResources.getDirectory(ResourceType.ZOO_MODEL, modelName())
			Dim cachedFile As New File(rootCacheDir, localFilename)

			If Not cachedFile.exists() Then
				log.info("Downloading model to " & cachedFile.ToString())
				FileUtils.copyURLToFile(New URL(remoteUrl), cachedFile,Integer.MaxValue,Integer.MaxValue)
			Else
				log.info("Using cached model at " & cachedFile.ToString())
			End If

			Dim expectedChecksum As Long = pretrainedChecksum(pretrainedType)
			If expectedChecksum <> 0L Then
				log.info("Verifying download...")
				Dim adler As Checksum = New Adler32()
				FileUtils.checksum(cachedFile, adler)
				Dim localChecksum As Long = adler.getValue()
				log.info("Checksum local is " & localChecksum & ", expecting " & expectedChecksum)

				If expectedChecksum <> localChecksum Then
					log.error("Checksums do not match. Cleaning up files and failing...")
					cachedFile.delete()
					Throw New System.InvalidOperationException("Pretrained model file failed checksum. If this error persists, please open an issue at https://github.com/eclipse/deeplearning4j.")
				End If
			End If

			If modelType() = GetType(MultiLayerNetwork) Then
				Return CType(ModelSerializer.restoreMultiLayerNetwork(cachedFile), M)
			ElseIf modelType() = GetType(ComputationGraph) Then
				Return CType(ModelSerializer.restoreComputationGraph(cachedFile), M)
			Else
				Throw New System.NotSupportedException("Pretrained models are only supported for MultiLayerNetwork and ComputationGraph.")
			End If
		End Function

		Public Overridable Function modelName() As String Implements InstantiableModel.modelName
			Return Me.GetType().Name.ToLower()
		End Function
	End Class

End Namespace