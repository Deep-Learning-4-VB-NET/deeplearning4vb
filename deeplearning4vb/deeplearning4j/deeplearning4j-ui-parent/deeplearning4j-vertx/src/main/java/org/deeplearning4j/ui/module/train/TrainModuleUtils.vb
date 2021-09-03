Imports System
Imports System.Collections.Generic
Imports JsonIgnore = com.fasterxml.jackson.annotation.JsonIgnore
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports GraphVertex = org.deeplearning4j.nn.conf.graph.GraphVertex
Imports LayerVertex = org.deeplearning4j.nn.conf.graph.LayerVertex
Imports org.deeplearning4j.nn.conf.layers
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports VariationalAutoencoderParamInitializer = org.deeplearning4j.nn.params.VariationalAutoencoderParamInitializer

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

Namespace org.deeplearning4j.ui.module.train


	Public Class TrainModuleUtils


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public static class GraphInfo
		Public Class GraphInfo

			Friend vertexNames As IList(Of String)
			Friend vertexTypes As IList(Of String)
			Friend vertexInputs As IList(Of IList(Of Integer))
			Friend vertexInfo As IList(Of IDictionary(Of String, String))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore private List<String> originalVertexName;
			Friend originalVertexName As IList(Of String)
		End Class

		Public Shared Function buildGraphInfo(ByVal config As MultiLayerConfiguration) As GraphInfo
			Dim vertexNames As IList(Of String) = New List(Of String)()
			Dim originalVertexName As IList(Of String) = New List(Of String)()
			Dim layerTypes As IList(Of String) = New List(Of String)()
			Dim layerInputs As IList(Of IList(Of Integer)) = New List(Of IList(Of Integer))()
			Dim layerInfo As IList(Of IDictionary(Of String, String)) = New List(Of IDictionary(Of String, String))()
			vertexNames.Add("Input")
			originalVertexName.Add(Nothing)
			layerTypes.Add("Input")
			layerInputs.Add(java.util.Collections.emptyList())
			layerInfo.Add(java.util.Collections.emptyMap())


			Dim list As IList(Of NeuralNetConfiguration) = config.getConfs()
			Dim layerIdx As Integer = 1
			For Each c As NeuralNetConfiguration In list
				Dim layer As Layer = c.getLayer()
				Dim layerName As String = layer.LayerName
				If layerName Is Nothing Then
					layerName = "layer" & layerIdx
				End If
				vertexNames.Add(layerName)
				originalVertexName.Add((layerIdx - 1).ToString())

				Dim layerType As String = c.getLayer().GetType().Name.replaceAll("Layer$", "")
				layerTypes.Add(layerType)

				layerInputs.Add(Collections.singletonList(layerIdx - 1))
				layerIdx += 1

				'Extract layer info
				Dim map As IDictionary(Of String, String) = getLayerInfo(c, layer)
				layerInfo.Add(map)
			Next c

			Return New GraphInfo(vertexNames, layerTypes, layerInputs, layerInfo, originalVertexName)
		End Function

		Public Shared Function buildGraphInfo(ByVal config As ComputationGraphConfiguration) As GraphInfo
			Dim layerNames As IList(Of String) = New List(Of String)()
			Dim layerTypes As IList(Of String) = New List(Of String)()
			Dim layerInputs As IList(Of IList(Of Integer)) = New List(Of IList(Of Integer))()
			Dim layerInfo As IList(Of IDictionary(Of String, String)) = New List(Of IDictionary(Of String, String))()


			Dim vertices As IDictionary(Of String, GraphVertex) = config.getVertices()
			Dim vertexInputs As IDictionary(Of String, IList(Of String)) = config.getVertexInputs()
			Dim networkInputs As IList(Of String) = config.getNetworkInputs()

			Dim originalVertexName As IList(Of String) = New List(Of String)()

			Dim vertexToIndexMap As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
			Dim vertexCount As Integer = 0
			For Each s As String In networkInputs
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: vertexToIndexMap.put(s, vertexCount++);
				vertexToIndexMap(s) = vertexCount
					vertexCount += 1
				layerNames.Add(s)
				originalVertexName.Add(s)
				layerTypes.Add(s)
				layerInputs.Add(java.util.Collections.emptyList())
				layerInfo.Add(java.util.Collections.emptyMap())
			Next s

			For Each s As String In vertices.Keys
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: vertexToIndexMap.put(s, vertexCount++);
				vertexToIndexMap(s) = vertexCount
					vertexCount += 1
			Next s

			For Each entry As KeyValuePair(Of String, GraphVertex) In vertices.SetOfKeyValuePairs()
				Dim gv As GraphVertex = entry.Value
				layerNames.Add(entry.Key)

				Dim inputsThisVertex As IList(Of String) = vertexInputs(entry.Key)
				Dim inputIndexes As IList(Of Integer) = New List(Of Integer)()
				For Each s As String In inputsThisVertex
					inputIndexes.Add(vertexToIndexMap(s))
				Next s

				layerInputs.Add(inputIndexes)

				If TypeOf gv Is LayerVertex Then
					Dim c As NeuralNetConfiguration = DirectCast(gv, LayerVertex).getLayerConf()
					Dim layer As Layer = c.getLayer()

					Dim layerType As String = layer.GetType().Name.replaceAll("Layer$", "")
					layerTypes.Add(layerType)

					'Extract layer info
					Dim map As IDictionary(Of String, String) = getLayerInfo(c, layer)
					layerInfo.Add(map)
				Else
					Dim layerType As String = gv.GetType().Name
					layerTypes.Add(layerType)
					Dim thisVertexInfo As IDictionary(Of String, String) = java.util.Collections.emptyMap() 'TODO
					layerInfo.Add(thisVertexInfo)
				End If
				originalVertexName.Add(entry.Key)
			Next entry

			Return New GraphInfo(layerNames, layerTypes, layerInputs, layerInfo, originalVertexName)
		End Function

		Public Shared Function buildGraphInfo(ByVal config As NeuralNetConfiguration) As GraphInfo

			Dim vertexNames As IList(Of String) = New List(Of String)()
			Dim originalVertexName As IList(Of String) = New List(Of String)()
			Dim layerTypes As IList(Of String) = New List(Of String)()
			Dim layerInputs As IList(Of IList(Of Integer)) = New List(Of IList(Of Integer))()
			Dim layerInfo As IList(Of IDictionary(Of String, String)) = New List(Of IDictionary(Of String, String))()
			vertexNames.Add("Input")
			originalVertexName.Add(Nothing)
			layerTypes.Add("Input")
			layerInputs.Add(java.util.Collections.emptyList())
			layerInfo.Add(java.util.Collections.emptyMap())

			If TypeOf config.getLayer() Is VariationalAutoencoder Then
				'Special case like this is a bit ugly - but it works
				Dim va As VariationalAutoencoder = CType(config.getLayer(), VariationalAutoencoder)
				Dim encLayerSizes() As Integer = va.getEncoderLayerSizes()
				Dim decLayerSizes() As Integer = va.getDecoderLayerSizes()

				Dim layerIndex As Integer = 1
				For i As Integer = 0 To encLayerSizes.Length - 1
					Dim name As String = "encoder_" & i
					vertexNames.Add(name)
					originalVertexName.Add("e" & i)
					Dim layerType As String = "VAE-Encoder"
					layerTypes.Add(layerType)
					layerInputs.Add(Collections.singletonList(layerIndex - 1))
					layerIndex += 1

					Dim encoderInfo As IDictionary(Of String, String) = New LinkedHashMap(Of String, String)()
					Dim inputSize As Long = (If(i = 0, va.getNIn(), encLayerSizes(i - 1)))
					Dim outputSize As Long = encLayerSizes(i)
					encoderInfo("Input Size") = inputSize.ToString()
					encoderInfo("Layer Size") = outputSize.ToString()
					encoderInfo("Num Parameters") = ((inputSize + 1) * outputSize).ToString()
					encoderInfo("Activation Function") = va.getActivationFn().ToString()
					layerInfo.Add(encoderInfo)
				Next i

				vertexNames.Add("z")
				originalVertexName.Add(VariationalAutoencoderParamInitializer.PZX_PREFIX)
				layerTypes.Add("VAE-LatentVariable")
				layerInputs.Add(Collections.singletonList(layerIndex - 1))
				layerIndex += 1
				Dim latentInfo As IDictionary(Of String, String) = New LinkedHashMap(Of String, String)()
				Dim inputSize As Long = encLayerSizes(encLayerSizes.Length - 1)
				Dim outputSize As Long = va.getNOut()
				latentInfo("Input Size") = inputSize.ToString()
				latentInfo("Layer Size") = outputSize.ToString()
				latentInfo("Num Parameters") = ((inputSize + 1) * outputSize * 2).ToString()
				latentInfo("Activation Function") = va.getPzxActivationFn().ToString()
				layerInfo.Add(latentInfo)


				For i As Integer = 0 To decLayerSizes.Length - 1
					Dim name As String = "decoder_" & i
					vertexNames.Add(name)
					originalVertexName.Add("d" & i)
					Dim layerType As String = "VAE-Decoder"
					layerTypes.Add(layerType)
					layerInputs.Add(Collections.singletonList(layerIndex - 1))
					layerIndex += 1

					Dim decoderInfo As IDictionary(Of String, String) = New LinkedHashMap(Of String, String)()
					inputSize = (If(i = 0, va.getNOut(), decLayerSizes(i - 1)))
					outputSize = decLayerSizes(i)
					decoderInfo("Input Size") = inputSize.ToString()
					decoderInfo("Layer Size") = outputSize.ToString()
					decoderInfo("Num Parameters") = ((inputSize + 1) * outputSize).ToString()
					decoderInfo("Activation Function") = va.getActivationFn().ToString()
					layerInfo.Add(decoderInfo)
				Next i

				vertexNames.Add("x")
				originalVertexName.Add(VariationalAutoencoderParamInitializer.PXZ_PREFIX)
				layerTypes.Add("VAE-Reconstruction")
				layerInputs.Add(Collections.singletonList(layerIndex - 1))
				layerIndex += 1
				Dim reconstructionInfo As IDictionary(Of String, String) = New LinkedHashMap(Of String, String)()
				inputSize = decLayerSizes(decLayerSizes.Length - 1)
				outputSize = va.getNIn()
				reconstructionInfo("Input Size") = inputSize.ToString()
				reconstructionInfo("Layer Size") = outputSize.ToString()
				reconstructionInfo("Num Parameters") = ((inputSize + 1) * va.getOutputDistribution().distributionInputSize(CInt(Math.Truncate(va.getNIn())))).ToString()
				reconstructionInfo("Distribution") = va.getOutputDistribution().ToString()
				layerInfo.Add(reconstructionInfo)


			Else
				'VAE or similar...
				Dim layer As Layer = config.getLayer()
				Dim layerName As String = layer.LayerName
				If layerName Is Nothing Then
					layerName = "layer0"
				End If
				vertexNames.Add(layerName)
				originalVertexName.Add("0".ToString())

				Dim layerType As String = config.getLayer().GetType().Name.replaceAll("Layer$", "")
				layerTypes.Add(layerType)

				layerInputs.Add(Collections.singletonList(0))

				'Extract layer info
				Dim map As IDictionary(Of String, String) = getLayerInfo(config, layer)
				layerInfo.Add(map)
			End If


			Return New GraphInfo(vertexNames, layerTypes, layerInputs, layerInfo, originalVertexName)
		End Function


		Private Shared Function getLayerInfo(ByVal c As NeuralNetConfiguration, ByVal layer As Layer) As IDictionary(Of String, String)
			Dim map As IDictionary(Of String, String) = New LinkedHashMap(Of String, String)()

			If TypeOf layer Is FeedForwardLayer Then
				Dim layer1 As FeedForwardLayer = DirectCast(layer, FeedForwardLayer)
				map("Input size") = layer1.getNIn().ToString()
				map("Output size") = layer1.getNOut().ToString()
				map("Num Parameters") = (layer1.initializer().numParams(c)).ToString()
				map("Activation Function") = layer1.getActivationFn().ToString()
			End If

			If TypeOf layer Is ConvolutionLayer Then
				Dim layer1 As ConvolutionLayer = DirectCast(layer, ConvolutionLayer)
				map("Kernel size") = java.util.Arrays.toString(layer1.getKernelSize())
				map("Stride") = java.util.Arrays.toString(layer1.getStride())
				map("Padding") = java.util.Arrays.toString(layer1.getPadding())
			ElseIf TypeOf layer Is SubsamplingLayer Then
				Dim layer1 As SubsamplingLayer = DirectCast(layer, SubsamplingLayer)
				map("Kernel size") = java.util.Arrays.toString(layer1.getKernelSize())
				map("Stride") = java.util.Arrays.toString(layer1.getStride())
				map("Padding") = java.util.Arrays.toString(layer1.getPadding())
				map("Pooling Type") = layer1.getPoolingType().ToString()
			ElseIf TypeOf layer Is BaseOutputLayer Then
				Dim ol As BaseOutputLayer = DirectCast(layer, BaseOutputLayer)
				If ol.getLossFn() IsNot Nothing Then
					map("Loss Function") = ol.getLossFn().ToString()
				End If
			End If

			Return map
		End Function
	End Class

End Namespace