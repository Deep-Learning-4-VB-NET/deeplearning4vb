Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports System.Linq
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports TFGraphRunnerService = org.nd4j.TFGraphRunnerService
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
Imports Gson = com.google.gson.Gson
Imports Message = org.nd4j.shade.protobuf.Message
Imports TextFormat = org.nd4j.shade.protobuf.TextFormat

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

Namespace org.deeplearning4j.nn.modelimport.keras.layers



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Data public class TFOpLayerImpl extends org.deeplearning4j.nn.layers.AbstractLayer<TFOpLayer>
	<Serializable>
	Public Class TFOpLayerImpl
		Inherits AbstractLayer(Of TFOpLayer)


		Private nodeDef As System.Collections.IDictionary
		Private constants As System.Collections.IDictionary
		Private inputNames As IList(Of String)
		Friend graphRunnerService As TFGraphRunnerService

		Public Sub New(ByVal nodeDef As System.Collections.IDictionary, ByVal constants As System.Collections.IDictionary, ByVal conf As NeuralNetConfiguration, ByVal dtype As DataType)
			MyBase.New(conf, dtype)
			Me.nodeDef = nodeDef
			Me.constants = constants
			setGraphRunner()
		End Sub

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			Throw New Exception("Backprop through TFOpLayerImpl is not supported yet." & " TFOpLayerImpl is created when importing TensorFlow 2.0 Keras models " & "(tf.keras) into DL4J, that contains TensorFlow operations not just Keras layers.")
		End Function

		''' <summary>
		''' Converts a Map representation of Nodedef to a singleton TF Graph and instantiates a GraphRunner.
		''' </summary>
		Private Sub setGraphRunner()
			Try
				Dim json As String = (New Gson()).toJson(nodeDef)
				Dim builder As NodeDef.Builder = NodeDef.newBuilder()
				org.nd4j.shade.protobuf.util.JsonFormat.parser().merge(json, builder)
				Dim nodeDef As NodeDef = builder.build()
				Dim allInputNames As IList(Of String) = New List(Of String)() ' including constants
				Dim inputDataTypes As IDictionary(Of String, String) = New Dictionary(Of String, String)()
				Dim constArrays As IDictionary(Of String, INDArray) = New Hashtable()
				Me.inputNames = New List(Of String)()
				Dim outputNames As IList(Of String) = New List(Of String) From {nodeDef.getName()}
				Dim attrMap As IDictionary(Of String, AttrValue) = nodeDef.getAttrMap()
				Dim i As Integer = 0
				Do While i < nodeDef.getInputCount()
					Dim inputName As String = nodeDef.getInput(i)
					Dim split() As String = inputName.Split("/", True)
					Dim attrKey As String
					If split.Length = 1 Then
						attrKey = "T"
					Else
						attrKey = "T" & split(split.Length - 1)
					End If
					allInputNames.Add(nodeDef.getInput(i))
					inputDataTypes(nodeDef.getInput(i)) = attrMap(attrKey).getType().ToString()
					If constants.Contains(i.ToString()) Then
						constArrays(nodeDef.getInput(i)) = Nd4j.create(CType(constants(i.ToString()), IList(Of Number)))
					Else
						Me.inputNames.Add(nodeDef.getInput(i))
					End If
					i += 1
				Loop
				Dim graph As String = "node{" & vbLf & nodeDef.ToString() & vbLf & "}" & vbLf & "versions {" & vbLf & " producer: 22" & vbLf & "}"
				For i As Integer = 0 To allInputNames.Count - 1
					Dim inpName As String = allInputNames(i)
					Dim dtype As String = inputDataTypes(inpName)
					graph = "node{" & vbLf & "name: """ & inpName & """" & vbLf & "op: ""Placeholder""" & vbLf & "attr{" & vbLf & "key: ""dtype""" & vbLf & " value {" & vbLf & " type: " & dtype & "}" & vbLf & "}" & vbLf & "}" & vbLf & graph
				Next i
				'log.info(graph);
				Dim graphDefBuilder As GraphDef.Builder = GraphDef.newBuilder()
				TextFormat.getParser().merge(graph, graphDefBuilder)
				Dim graphDef As GraphDef = graphDefBuilder.build()
				Dim serialized As org.nd4j.shade.protobuf.ByteString = graphDef.toByteString()
				Dim graphBytes() As SByte = serialized.toByteArray()

				Dim sl As ServiceLoader(Of TFGraphRunnerService) = DL4JClassLoading.loadService(GetType(TFGraphRunnerService))
				Dim iter As IEnumerator(Of TFGraphRunnerService) = sl.GetEnumerator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If Not iter.hasNext() Then
					Throw New Exception("The model contains a Tensorflow Op, which requires the nd4j-tensorflow dependency to execute.")
				End If

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Me.graphRunnerService = iter.next().init(allInputNames, outputNames, graphBytes, constArrays, inputDataTypes)
			Catch e As Exception
				Throw New Exception("Error parsing protobuf", e)
			End Try

		End Sub

		Private Function runGraph(ByVal input As INDArray) As INDArray
			Dim inputMap As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			inputMap(inputNames(0)) = input
			Dim [out] As INDArray = graphRunnerService.run(inputMap).Values.ToArray()(0)
			Return [out]
		End Function

		Public Overridable Function getOutputShape(ByVal inputShape() As Long) As Long()
			Dim shape() As Long = ArrayUtils.clone(inputShape)
			For i As Integer = 0 To shape.Length - 1
				If shape(i) < 0 Then
					shape(i) = 1
				End If
			Next i
			Dim dummyArr As INDArray = Nd4j.zeros(shape)
			Return runGraph(dummyArr).shape()
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Return runGraph(input_Conflict)
		End Function


		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Sub clearNoiseWeightParams()

		End Sub

	End Class

End Namespace