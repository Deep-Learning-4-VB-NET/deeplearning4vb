Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports org.deeplearning4j.nn.conf
Imports Distribution = org.deeplearning4j.nn.conf.distribution.Distribution
Imports GraphVertex = org.deeplearning4j.nn.conf.graph.GraphVertex
Imports LayerVertex = org.deeplearning4j.nn.conf.graph.LayerVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports FeedForwardLayer = org.deeplearning4j.nn.conf.layers.FeedForwardLayer
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports VertexIndices = org.deeplearning4j.nn.graph.vertex.VertexIndices
Imports FrozenVertex = org.deeplearning4j.nn.graph.vertex.impl.FrozenVertex
Imports InputVertex = org.deeplearning4j.nn.graph.vertex.impl.InputVertex
Imports FrozenLayer = org.deeplearning4j.nn.layers.FrozenLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports WeightInitDistribution = org.deeplearning4j.nn.weights.WeightInitDistribution
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.nn.transferlearning


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class TransferLearning
	Public Class TransferLearning

		Public Class Builder
			Friend origConf As MultiLayerConfiguration
			Friend origModel As MultiLayerNetwork

			Friend editedModel As MultiLayerNetwork
'JAVA TO VB CONVERTER NOTE: The field finetuneConfiguration was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend finetuneConfiguration_Conflict As FineTuneConfiguration
			Friend frozenTill As Integer = -1
			Friend popN As Integer = 0
			Friend prepDone As Boolean = False
			Friend editedLayers As ISet(Of Integer) = New HashSet(Of Integer)()
			Friend editedLayersMap As IDictionary(Of Integer, Triple(Of Integer, IWeightInit, IWeightInit)) = New Dictionary(Of Integer, Triple(Of Integer, IWeightInit, IWeightInit))()
			Friend nInEditedMap As IDictionary(Of Integer, Pair(Of Integer, IWeightInit)) = New Dictionary(Of Integer, Pair(Of Integer, IWeightInit))()
			Friend editedParams As IList(Of INDArray) = New List(Of INDArray)()
			Friend editedConfs As IList(Of NeuralNetConfiguration) = New List(Of NeuralNetConfiguration)()
			Friend appendParams As IList(Of INDArray) = New List(Of INDArray)() 'these could be new arrays, and views from origParams
			Friend appendConfs As IList(Of NeuralNetConfiguration) = New List(Of NeuralNetConfiguration)()

			Friend inputPreProcessors As IDictionary(Of Integer, InputPreProcessor) = New Dictionary(Of Integer, InputPreProcessor)()

			Friend inputType As InputType
'JAVA TO VB CONVERTER NOTE: The field validateOutputLayerConfig was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend validateOutputLayerConfig_Conflict As Boolean?
			Friend dataType As DataType

			''' <summary>
			''' Multilayer Network to tweak for transfer learning </summary>
			''' <param name="origModel"> </param>
			Public Sub New(ByVal origModel As MultiLayerNetwork)
				Me.origModel = origModel
				Me.origConf = origModel.LayerWiseConfigurations.clone()
				Me.dataType = origModel.LayerWiseConfigurations.getDataType()

				Me.inputPreProcessors = origConf.getInputPreProcessors()
			End Sub

			''' <summary>
			''' Fine tune configurations specified will overwrite the existing configuration if any
			''' Usage example: specify a learning rate will set specified learning rate on all layers
			''' Refer to the fineTuneConfiguration class for more details </summary>
			''' <param name="finetuneConfiguration"> </param>
			''' <returns> Builder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter finetuneConfiguration was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function fineTuneConfiguration(ByVal finetuneConfiguration_Conflict As FineTuneConfiguration) As Builder
				Me.finetuneConfiguration_Conflict = finetuneConfiguration_Conflict
				Return Me
			End Function

			''' <summary>
			''' Specify a layer to set as a "feature extractor"
			''' The specified layer and the layers preceding it will be "frozen" with parameters staying constant </summary>
			''' <param name="layerNum"> </param>
			''' <returns> Builder </returns>
			Public Overridable Function setFeatureExtractor(ByVal layerNum As Integer) As Builder
				Me.frozenTill = layerNum
				Return Me
			End Function

			''' <summary>
			''' Modify the architecture of a layer by changing nOut
			''' Note this will also affect the layer that follows the layer specified, unless it is the output layer
			''' </summary>
			''' <param name="layerNum"> The index of the layer to change nOut of </param>
			''' <param name="nOut">     Value of nOut to change to </param>
			''' <param name="scheme">   Weight Init scheme to use for params in layernum and layernum+1 </param>
			''' <returns> Builder </returns>
			Public Overridable Function nOutReplace(ByVal layerNum As Integer, ByVal nOut As Integer, ByVal scheme As WeightInit) As Builder
				Return nOutReplace(layerNum, nOut, scheme, scheme)
			End Function

			''' <summary>
			''' Modify the architecture of a layer by changing nOut
			''' Note this will also affect the layer that follows the layer specified, unless it is the output layer
			''' </summary>
			''' <param name="layerNum"> The index of the layer to change nOut of </param>
			''' <param name="nOut">     Value of nOut to change to </param>
			''' <param name="dist">     Distribution to use in conjunction with weight init DISTRIBUTION for params in layernum and layernum+1 </param>
			''' <returns> Builder </returns>
			''' <seealso cref= WeightInit DISTRIBUTION </seealso>
			Public Overridable Function nOutReplace(ByVal layerNum As Integer, ByVal nOut As Integer, ByVal dist As Distribution) As Builder
				Return nOutReplace(layerNum, nOut, New WeightInitDistribution(dist), New WeightInitDistribution(dist))
			End Function

			''' <summary>
			''' Modify the architecture of a layer by changing nOut
			''' Note this will also affect the layer that follows the layer specified, unless it is the output layer
			''' Can specify different weight init schemes for the specified layer and the layer that follows it.
			''' </summary>
			''' <param name="layerNum">   The index of the layer to change nOut of </param>
			''' <param name="nOut">       Value of nOut to change to </param>
			''' <param name="scheme">     Weight Init scheme to use for params in the layerNum </param>
			''' <param name="schemeNext"> Weight Init scheme to use for params in the layerNum+1 </param>
			''' <returns> Builder </returns>
			Public Overridable Function nOutReplace(ByVal layerNum As Integer, ByVal nOut As Integer, ByVal scheme As WeightInit, ByVal schemeNext As WeightInit) As Builder
				If scheme = WeightInit.DISTRIBUTION OrElse schemeNext = WeightInit.DISTRIBUTION Then
					Throw New System.NotSupportedException("Not supported!, Use " & "nOutReplace(layerNum, nOut, new WeightInitDistribution(dist), new WeightInitDistribution(distNext)) instead!")
				End If
				Return nOutReplace(layerNum, nOut, scheme.getWeightInitFunction(), schemeNext.getWeightInitFunction())
			End Function

			''' <summary>
			''' Modify the architecture of a layer by changing nOut
			''' Note this will also affect the layer that follows the layer specified, unless it is the output layer
			''' Can specify different weight init schemes for the specified layer and the layer that follows it.
			''' </summary>
			''' <param name="layerNum"> The index of the layer to change nOut of </param>
			''' <param name="nOut">     Value of nOut to change to </param>
			''' <param name="dist">     Distribution to use for params in the layerNum </param>
			''' <param name="distNext"> Distribution to use for parmas in layerNum+1 </param>
			''' <returns> Builder </returns>
			''' <seealso cref= WeightInitDistribution </seealso>
			Public Overridable Function nOutReplace(ByVal layerNum As Integer, ByVal nOut As Integer, ByVal dist As Distribution, ByVal distNext As Distribution) As Builder
				Return nOutReplace(layerNum, nOut, New WeightInitDistribution(dist), New WeightInitDistribution(distNext))
			End Function

			''' <summary>
			''' Modify the architecture of a layer by changing nOut
			''' Note this will also affect the layer that follows the layer specified, unless it is the output layer
			''' Can specify different weight init schemes for the specified layer and the layer that follows it.
			''' </summary>
			''' <param name="layerNum"> The index of the layer to change nOut of </param>
			''' <param name="nOut">     Value of nOut to change to </param>
			''' <param name="scheme">   Weight init scheme to use for params in layerNum </param>
			''' <param name="distNext"> Distribution to use for parmas in layerNum+1 </param>
			''' <returns> Builder </returns>
			''' <seealso cref= WeightInitDistribution </seealso>
			Public Overridable Function nOutReplace(ByVal layerNum As Integer, ByVal nOut As Integer, ByVal scheme As WeightInit, ByVal distNext As Distribution) As Builder
				If scheme = WeightInit.DISTRIBUTION Then
					Throw New System.NotSupportedException("Not supported!, Use " & "nOutReplace(int layerNum, int nOut, Distribution dist, Distribution distNext) instead!")
				End If
				Return nOutReplace(layerNum, nOut, scheme.getWeightInitFunction(), New WeightInitDistribution(distNext))
			End Function

			''' <summary>
			''' Modify the architecture of a layer by changing nOut
			''' Note this will also affect the layer that follows the layer specified, unless it is the output layer
			''' Can specify different weight init schemes for the specified layer and the layer that follows it.
			''' </summary>
			''' <param name="layerNum">   The index of the layer to change nOut of </param>
			''' <param name="nOut">       Value of nOut to change to </param>
			''' <param name="dist">       Distribution to use for parmas in layerNum </param>
			''' <param name="schemeNext"> Weight init scheme to use for params in layerNum+1 </param>
			''' <returns> Builder </returns>
			''' <seealso cref= WeightInitDistribution </seealso>
			Public Overridable Function nOutReplace(ByVal layerNum As Integer, ByVal nOut As Integer, ByVal dist As Distribution, ByVal schemeNext As WeightInit) As Builder
				Return nOutReplace(layerNum, nOut, New WeightInitDistribution(dist), schemeNext.getWeightInitFunction())
			End Function

			''' <summary>
			''' Modify the architecture of a layer by changing nOut
			''' Note this will also affect the layer that follows the layer specified, unless it is the output layer
			''' Can specify different weight init schemes for the specified layer and the layer that follows it.
			''' </summary>
			''' <param name="layerNum">   The index of the layer to change nOut of </param>
			''' <param name="nOut">       Value of nOut to change to </param>
			''' <param name="scheme">     Weight Init scheme to use for params in the layerNum </param>
			''' <param name="schemeNext"> Weight Init scheme to use for params in the layerNum+1 </param>
			Public Overridable Function nOutReplace(ByVal layerNum As Integer, ByVal nOut As Integer, ByVal scheme As IWeightInit, ByVal schemeNext As IWeightInit) As Builder
				editedLayers.Add(layerNum)
				Dim t As New Triple(Of Integer, IWeightInit, IWeightInit)(nOut, scheme, schemeNext)
				editedLayersMap(layerNum) = t
				Return Me
			End Function

			''' <summary>
			''' Modify the architecture of a vertex layer by changing nIn of the specified layer.<br>
			''' Note that only the specified layer will be modified - all other layers will not be changed by this call.
			''' </summary>
			''' <param name="layerNum"> The number of the layer to change nIn of </param>
			''' <param name="nIn">      Value of nIn to change to </param>
			''' <param name="scheme">   Weight init scheme to use for params in layerName </param>
			''' <returns> Builder </returns>
			Public Overridable Function nInReplace(ByVal layerNum As Integer, ByVal nIn As Integer, ByVal scheme As WeightInit) As Builder
				Return nInReplace(layerNum, nIn, scheme, Nothing)
			End Function

			''' <summary>
			''' Modify the architecture of a vertex layer by changing nIn of the specified layer.<br>
			''' Note that only the specified layer will be modified - all other layers will not be changed by this call.
			''' </summary>
			''' <param name="layerNum"> The number of the layer to change nIn of </param>
			''' <param name="nIn">      Value of nIn to change to </param>
			''' <param name="scheme">   Weight init scheme to use for params in layerName </param>
			''' <returns> Builder </returns>
			Public Overridable Function nInReplace(ByVal layerNum As Integer, ByVal nIn As Integer, ByVal scheme As WeightInit, ByVal dist As Distribution) As Builder
				Return nInReplace(layerNum, nIn, scheme.getWeightInitFunction(dist))
			End Function

			''' <summary>
			''' Modify the architecture of a vertex layer by changing nIn of the specified layer.<br>
			''' Note that only the specified layer will be modified - all other layers will not be changed by this call.
			''' </summary>
			''' <param name="layerNum"> The number of the layer to change nIn of </param>
			''' <param name="nIn">      Value of nIn to change to </param>
			''' <param name="scheme">   Weight init scheme to use for params in layerName </param>
			''' <returns> Builder </returns>
			Public Overridable Function nInReplace(ByVal layerNum As Integer, ByVal nIn As Integer, ByVal scheme As IWeightInit) As Builder
				Dim d As New Pair(Of Integer, IWeightInit)(nIn, scheme)
				nInEditedMap(layerNum) = d
				Return Me
			End Function

			''' <summary>
			''' Helper method to remove the outputLayer of the net.
			''' Only one of the two - removeOutputLayer() or removeLayersFromOutput(layerNum) - can be specified
			''' When removing layers at the very least an output layer should be added with .addLayer(...)
			''' </summary>
			''' <returns> Builder </returns>
			Public Overridable Function removeOutputLayer() As Builder
				popN = 1
				Return Me
			End Function

			''' <summary>
			''' Remove last "n" layers of the net
			''' At least an output layer must be added back in </summary>
			''' <param name="layerNum"> number of layers to remove </param>
			''' <returns> Builder </returns>
			Public Overridable Function removeLayersFromOutput(ByVal layerNum As Integer) As Builder
				If popN = 0 Then
					popN = layerNum
				Else
					Throw New System.ArgumentException("Remove layers from can only be called once")
				End If
				Return Me
			End Function

			''' <summary>
			''' Add layers to the net
			''' Required if layers are removed. Can be called multiple times and layers will be added in the order with which they were called.
			''' At the very least an outputLayer must be added (output layer should be added last - as per the note on order)
			''' Learning configs (like updaters, learning rate etc) specified with the layer here will be honored
			''' </summary>
			''' <param name="layer"> layer conf to add (similar to the NeuralNetConfiguration .list().layer(...) </param>
			''' <returns> Builder </returns>
			Public Overridable Function addLayer(ByVal layer As Layer) As Builder

				If Not prepDone Then
					doPrep()
				End If

				' Use the fineTune config to create the required NeuralNetConfiguration + Layer instances
				'instantiate dummy layer to get the params

				'Build a nn config builder with settings from finetune. Set layer with the added layer
				'Issue: fine tune config has .learningRate(x), then I add a layer with .learningRate(y)...
				'We don't want that to be overridden
				Dim layerConf As NeuralNetConfiguration = finetuneConfiguration_Conflict.appliedNeuralNetConfigurationBuilder().layer(layer).build()

				Dim numParams As val = layer.initializer().numParams(layerConf)
				Dim params As INDArray
				If numParams > 0 Then
					params = Nd4j.create(origModel.LayerWiseConfigurations.getDataType(), 1, numParams)
					Dim someLayer As org.deeplearning4j.nn.api.Layer = layer.instantiate(layerConf, Nothing, 0, params, True, dataType)
					appendParams.Add(someLayer.params())
					appendConfs.Add(someLayer.conf())
				Else
					appendConfs.Add(layerConf)

				End If
				Return Me
			End Function

			''' <summary>
			''' Specify the preprocessor for the added layers
			''' for cases where they cannot be inferred automatically.
			''' </summary>
			''' <param name="processor"> to be used on the data </param>
			''' <returns> Builder </returns>
			Public Overridable Function setInputPreProcessor(ByVal layer As Integer, ByVal processor As InputPreProcessor) As Builder
				inputPreProcessors(layer) = processor
				Return Me
			End Function

			Public Overridable Function validateOutputLayerConfig(ByVal validate As Boolean) As Builder
				Me.validateOutputLayerConfig_Conflict = validate
				Return Me
			End Function

			''' <summary>
			''' Returns a model with the fine tune configuration and specified architecture changes.
			''' .init() need not be called. Can be directly fit.
			''' </summary>
			''' <returns> MultiLayerNetwork </returns>
			Public Overridable Function build() As MultiLayerNetwork

				If Not prepDone Then
					doPrep()
				End If

				editedModel = New MultiLayerNetwork(constructConf(), constructParams())
				If frozenTill <> -1 Then
					Dim layers() As org.deeplearning4j.nn.api.Layer = editedModel.Layers
					For i As Integer = frozenTill To 0 Step -1
						'Complication here: inner Layer (implementation) NeuralNetConfiguration.layer (config) should keep
						' the original layer config. While network NNC should have the frozen layer, for to/from JSON etc
						Dim origNNC As NeuralNetConfiguration = editedModel.LayerWiseConfigurations.getConf(i)
						Dim layerNNC As NeuralNetConfiguration = origNNC.clone()
						layers(i).Conf = layerNNC
						layers(i) = New FrozenLayer(layers(i))

						If origNNC.getVariables() IsNot Nothing Then
							Dim vars As IList(Of String) = origNNC.variables(True)
							origNNC.clearVariables()
							layerNNC.clearVariables()
							For Each s As String In vars
								origNNC.variables(False).Add(s)
								layerNNC.variables(False).Add(s)
							Next s
						End If

						Dim origLayerConf As Layer = editedModel.LayerWiseConfigurations.getConf(i).getLayer()
						Dim newLayerConf As Layer = New org.deeplearning4j.nn.conf.layers.misc.FrozenLayer(origLayerConf)
						newLayerConf.setLayerName(origLayerConf.LayerName)
						editedModel.LayerWiseConfigurations.getConf(i).setLayer(newLayerConf)
					Next i
					editedModel.Layers = layers
				End If

				Return editedModel
			End Function

			Friend Overridable Sub doPrep()
				'first set finetune configs on all layers in model
				fineTuneConfigurationBuild()

				'editParams gets original model params
				Dim i As Integer = 0
				Do While i < origModel.getnLayers()
					If origModel.getLayer(i).numParams() > 0 Then
						'dup only if params are there
						editedParams.Add(origModel.getLayer(i).params().dup())
					Else
						editedParams.Add(origModel.getLayer(i).params())
					End If
					i += 1
				Loop
				'apply changes to nout/nin if any in sorted order and save to editedParams
				If editedLayers.Count > 0 Then
					Dim editedLayersSorted() As Integer? = editedLayers.ToArray()
					Array.Sort(editedLayersSorted)
					For i As Integer = 0 To editedLayersSorted.Length - 1
						Dim layerNum As Integer = editedLayersSorted(i)
						nOutReplaceBuild(layerNum, editedLayersMap(layerNum).getLeft(), editedLayersMap(layerNum).getMiddle(), editedLayersMap(layerNum).getRight())
					Next i
				End If

				If nInEditedMap.Count > 0 Then
					Dim editedLayersSorted() As Integer? = nInEditedMap.Keys.ToArray()
					Array.Sort(editedLayersSorted)
					For Each layerNum As Integer? In editedLayersSorted
						Dim d As Pair(Of Integer, IWeightInit) = nInEditedMap(layerNum)
						nInReplaceBuild(layerNum, d.First, d.Second)
					Next layerNum
				End If

				'finally pop layers specified
				Dim i As Integer = 0
				Do While i < popN
					Dim layerNum As Integer? = origModel.getnLayers() - i
					If inputPreProcessors.ContainsKey(layerNum) Then
						inputPreProcessors.Remove(layerNum)
					End If
					editedConfs.RemoveAt(editedConfs.Count - 1)
					editedParams.RemoveAt(editedParams.Count - 1)
					i += 1
				Loop
				prepDone = True

			End Sub


			Friend Overridable Sub fineTuneConfigurationBuild()

				Dim i As Integer = 0
				Do While i < origConf.getConfs().size()
					Dim layerConf As NeuralNetConfiguration
					If finetuneConfiguration_Conflict IsNot Nothing Then
						Dim nnc As NeuralNetConfiguration = origConf.getConf(i).clone()
						finetuneConfiguration_Conflict.applyToNeuralNetConfiguration(nnc)
						layerConf = nnc
					Else
						layerConf = origConf.getConf(i).clone()
					End If
					editedConfs.Add(layerConf)
					i += 1
				Loop
			End Sub

			Friend Overridable Sub nInReplaceBuild(ByVal layerNum As Integer, ByVal nIn As Integer, ByVal init As IWeightInit)
				Preconditions.checkArgument(layerNum >= 0 AndAlso layerNum < editedConfs.Count, "Invalid layer index: must be 0 to " & "numLayers-1 = %s includive, got %s", editedConfs.Count, layerNum)
				Dim layerConf As NeuralNetConfiguration = editedConfs(layerNum)
				Dim layerImpl As Layer = layerConf.getLayer() 'not a clone need to modify nOut in place
				Preconditions.checkArgument(TypeOf layerImpl Is FeedForwardLayer, "nInReplace can only be applide on FeedForward layers;" & "got layer of type %s", layerImpl.GetType().Name)
				Dim layerImplF As FeedForwardLayer = DirectCast(layerImpl, FeedForwardLayer)
				layerImplF.setWeightInitFn(init)
				layerImplF.setNIn(nIn)
				Dim numParams As Long = layerImpl.initializer().numParams(layerConf)
				Dim params As INDArray = Nd4j.create(origModel.LayerWiseConfigurations.getDataType(), 1, numParams)
				Dim someLayer As org.deeplearning4j.nn.api.Layer = layerImpl.instantiate(layerConf, Nothing, 0, params, True, dataType)
				editedParams(layerNum) = someLayer.params()
			End Sub


			Friend Overridable Sub nOutReplaceBuild(ByVal layerNum As Integer, ByVal nOut As Integer, ByVal scheme As IWeightInit, ByVal schemeNext As IWeightInit)
				Preconditions.checkArgument(layerNum >= 0 AndAlso layerNum < editedConfs.Count, "Invalid layer index: must be 0 to " & "numLayers-1 = %s includive, got %s", editedConfs.Count, layerNum)

				Dim layerConf As NeuralNetConfiguration = editedConfs(layerNum)
				Dim layerImpl As Layer = layerConf.getLayer() 'not a clone need to modify nOut in place
				Preconditions.checkArgument(TypeOf layerImpl Is FeedForwardLayer, "nOutReplace can only be applide on FeedForward layers;" & "got layer of type %s", layerImpl.GetType().Name)
				Dim layerImplF As FeedForwardLayer = DirectCast(layerImpl, FeedForwardLayer)
				layerImplF.setWeightInitFn(scheme)
				layerImplF.setNOut(nOut)
				Dim numParams As Long = layerImpl.initializer().numParams(layerConf)
				Dim params As INDArray = Nd4j.create(origModel.LayerWiseConfigurations.getDataType(), 1, numParams)
				Dim someLayer As org.deeplearning4j.nn.api.Layer = layerImpl.instantiate(layerConf, Nothing, 0, params, True, dataType)
				editedParams(layerNum) = someLayer.params()

				If layerNum + 1 < editedConfs.Count Then
					layerConf = editedConfs(layerNum + 1)
					layerImpl = layerConf.getLayer() 'modify in place
					If TypeOf layerImpl Is FeedForwardLayer Then
						layerImplF = DirectCast(layerImpl, FeedForwardLayer)
						layerImplF.setWeightInitFn(schemeNext)
						layerImplF.setNIn(nOut)
						numParams = layerImpl.initializer().numParams(layerConf)
						If numParams > 0 Then
							params = Nd4j.create(origModel.LayerWiseConfigurations.getDataType(), 1, numParams)
							someLayer = layerImpl.instantiate(layerConf, Nothing, 0, params, True, dataType)
							editedParams(layerNum + 1) = someLayer.params()
						End If
					End If
				End If

			End Sub

			Friend Overridable Function constructParams() As INDArray
				'some params will be null for subsampling etc
				Dim keepView As INDArray = Nothing
				For Each aParam As INDArray In editedParams
					If aParam IsNot Nothing Then
						If keepView Is Nothing Then
							keepView = aParam
						Else
							keepView = Nd4j.hstack(keepView, aParam)
						End If
					End If
				Next aParam
				If appendParams.Count > 0 Then
					Dim appendView As INDArray = Nd4j.hstack(appendParams)
					Return Nd4j.hstack(keepView, appendView)
				Else
					Return keepView
				End If
			End Function

			Friend Overridable Function constructConf() As MultiLayerConfiguration
				'use the editedConfs list to make a new config
				Dim allConfs As IList(Of NeuralNetConfiguration) = New List(Of NeuralNetConfiguration)()
				CType(allConfs, List(Of NeuralNetConfiguration)).AddRange(editedConfs)
				CType(allConfs, List(Of NeuralNetConfiguration)).AddRange(appendConfs)

				'Set default layer names, if not set - as per NeuralNetConfiguration.ListBuilder.build()
				Dim i As Integer = 0
				Do While i < allConfs.Count
					If allConfs(i).getLayer().getLayerName() Is Nothing Then
						allConfs(i).getLayer().setLayerName("layer" & i)
					End If
					i += 1
				Loop

				Dim conf As MultiLayerConfiguration = (New MultiLayerConfiguration.Builder()).inputPreProcessors(inputPreProcessors).setInputType(Me.inputType).confs(allConfs).validateOutputLayerConfig(If(validateOutputLayerConfig_Conflict Is Nothing, True, validateOutputLayerConfig_Conflict)).dataType(origConf.getDataType()).build()
				If finetuneConfiguration_Conflict IsNot Nothing Then
					finetuneConfiguration_Conflict.applyToMultiLayerConfiguration(conf)
				End If
				Return conf
			End Function
		End Class

		Public Class GraphBuilder
			Friend origGraph As ComputationGraph
			Friend origConfig As ComputationGraphConfiguration

'JAVA TO VB CONVERTER NOTE: The variable fineTuneConfiguration was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Friend fineTuneConfiguration_Conflict As FineTuneConfiguration
			Friend editedConfigBuilder As ComputationGraphConfiguration.GraphBuilder

			Friend frozenOutputAt() As String
			Friend hasFrozen As Boolean = False
			Friend editedVertices As ISet(Of String) = New HashSet(Of String)()
'JAVA TO VB CONVERTER NOTE: The field workspaceMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend workspaceMode_Conflict As WorkspaceMode
'JAVA TO VB CONVERTER NOTE: The field validateOutputLayerConfig was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend validateOutputLayerConfig_Conflict As Boolean? = Nothing

			Friend nInFromNewConfig As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()

			''' <summary>
			''' Computation Graph to tweak for transfer learning </summary>
			''' <param name="origGraph"> </param>
			Public Sub New(ByVal origGraph As ComputationGraph)
				Me.origGraph = origGraph
				Me.origConfig = origGraph.Configuration.clone()
			End Sub

			''' <summary>
			''' Set parameters to selectively override existing learning parameters
			''' Usage eg. specify a lower learning rate. This will get applied to all layers </summary>
			''' <param name="fineTuneConfiguration"> </param>
			''' <returns> GraphBuilder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter fineTuneConfiguration was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Public Overridable Function fineTuneConfiguration(ByVal fineTuneConfiguration_Conflict As FineTuneConfiguration) As GraphBuilder
				Me.fineTuneConfiguration_Conflict = fineTuneConfiguration_Conflict
				Me.editedConfigBuilder = New ComputationGraphConfiguration.GraphBuilder(origConfig, fineTuneConfiguration_Conflict.appliedNeuralNetConfigurationBuilder())

				Dim vertices As IDictionary(Of String, GraphVertex) = Me.editedConfigBuilder.getVertices()
				For Each gv As KeyValuePair(Of String, GraphVertex) In vertices.SetOfKeyValuePairs()
					If TypeOf gv.Value Is LayerVertex Then
						Dim lv As LayerVertex = CType(gv.Value, LayerVertex)
						Dim nnc As NeuralNetConfiguration = lv.getLayerConf().clone()
						fineTuneConfiguration_Conflict.applyToNeuralNetConfiguration(nnc)
						vertices(gv.Key) = New LayerVertex(nnc, lv.PreProcessor)
						nnc.getLayer().setLayerName(gv.Key)
					End If
				Next gv

				Return Me
			End Function

			''' <summary>
			''' Specify a layer vertex to set as a "feature extractor"
			''' The specified layer vertex and the layers on the path from an input vertex to it will be "frozen" with parameters staying constant </summary>
			''' <param name="layerName"> </param>
			''' <returns> Builder </returns>
			Public Overridable Function setFeatureExtractor(ParamArray ByVal layerName() As String) As GraphBuilder
				Me.hasFrozen = True
				Me.frozenOutputAt = layerName
				Return Me
			End Function

			''' <summary>
			''' Modify the architecture of a vertex layer by changing nOut
			''' Note this will also affect the vertex layer that follows the layer specified, unless it is the output layer
			''' Currently does not support modifying nOut of layers that feed into non-layer vertices like merge, subset etc
			''' To modify nOut for such vertices use remove vertex, followed by add vertex
			''' Can specify different weight init schemes for the specified layer and the layer that follows it.
			''' </summary>
			''' <param name="layerName"> The name of the layer to change nOut of </param>
			''' <param name="nOut">      Value of nOut to change to </param>
			''' <param name="scheme">    Weight init scheme to use for params in layerName and the layers following it </param>
			''' <returns> GraphBuilder </returns>
			''' <seealso cref= WeightInit DISTRIBUTION </seealso>
			Public Overridable Function nOutReplace(ByVal layerName As String, ByVal nOut As Integer, ByVal scheme As WeightInit) As GraphBuilder
				Return nOutReplace(layerName, nOut, scheme, scheme)
			End Function

			''' <summary>
			''' Modify the architecture of a vertex layer by changing nOut
			''' Note this will also affect the vertex layer that follows the layer specified, unless it is the output layer
			''' Currently does not support modifying nOut of layers that feed into non-layer vertices like merge, subset etc
			''' To modify nOut for such vertices use remove vertex, followed by add vertex
			''' Can specify different weight init schemes for the specified layer and the layer that follows it.
			''' </summary>
			''' <param name="layerName"> The name of the layer to change nOut of </param>
			''' <param name="nOut">      Value of nOut to change to </param>
			''' <param name="dist">      Weight distribution scheme to use </param>
			''' <returns> GraphBuilder </returns>
			''' <seealso cref= WeightInit DISTRIBUTION </seealso>
			Public Overridable Function nOutReplace(ByVal layerName As String, ByVal nOut As Integer, ByVal dist As Distribution) As GraphBuilder
				Return nOutReplace(layerName, nOut, dist, dist)
			End Function

			''' <summary>
			''' Modified nOut of specified layer. Also affects layers following layerName unless they are output layers </summary>
			''' <param name="layerName"> The name of the layer to change nOut of </param>
			''' <param name="nOut">      Value of nOut to change to </param>
			''' <param name="dist">      Weight distribution scheme to use for layerName </param>
			''' <param name="distNext">  Weight distribution scheme for layers following layerName </param>
			''' <returns> GraphBuilder </returns>
			''' <seealso cref= WeightInit DISTRIBUTION </seealso>
			Public Overridable Function nOutReplace(ByVal layerName As String, ByVal nOut As Integer, ByVal dist As Distribution, ByVal distNext As Distribution) As GraphBuilder
				Return nOutReplace(layerName, nOut, New WeightInitDistribution(dist), New WeightInitDistribution(distNext))
			End Function

			Public Overridable Function nOutReplace(ByVal layerName As String, ByVal nOut As Integer, ByVal scheme As WeightInit, ByVal dist As Distribution) As GraphBuilder
				If scheme = WeightInit.DISTRIBUTION Then
					Throw New System.NotSupportedException("Not supported!, Use " & "nOutReplace(layerNum, nOut, new WeightInitDistribution(dist), new WeightInitDistribution(distNext)) instead!")
				End If
				Return nOutReplace(layerName, nOut, scheme.getWeightInitFunction(), New WeightInitDistribution(dist))
			End Function

			Public Overridable Function nOutReplace(ByVal layerName As String, ByVal nOut As Integer, ByVal dist As Distribution, ByVal scheme As WeightInit) As GraphBuilder
				If scheme = WeightInit.DISTRIBUTION Then
					Throw New System.NotSupportedException("Not supported!, Use " & "nOutReplace(layerNum, nOut, new WeightInitDistribution(dist), new WeightInitDistribution(distNext)) instead!")
				End If
				Return nOutReplace(layerName, nOut, New WeightInitDistribution(dist), scheme.getWeightInitFunction())
			End Function


			Public Overridable Function nOutReplace(ByVal layerName As String, ByVal nOut As Integer, ByVal scheme As WeightInit, ByVal schemeNext As WeightInit) As GraphBuilder
				If scheme = WeightInit.DISTRIBUTION OrElse schemeNext = WeightInit.DISTRIBUTION Then
					Throw New System.NotSupportedException("Not supported!, Use " & "nOutReplace(layerNum, nOut, new WeightInitDistribution(dist), new WeightInitDistribution(distNext)) instead!")
				End If
				Return nOutReplace(layerName, nOut, scheme.getWeightInitFunction(), schemeNext.getWeightInitFunction())
			End Function

			''' <summary>
			''' Modify the architecture of a vertex layer by changing nIn of the specified layer.<br>
			''' Note that only the specified layer will be modified - all other layers will not be changed by this call.
			''' </summary>
			''' <param name="layerName"> The name of the layer to change nIn of </param>
			''' <param name="nIn">      Value of nIn to change to </param>
			''' <param name="scheme">    Weight init scheme to use for params in layerName </param>
			''' <returns> GraphBuilder </returns>
			Public Overridable Function nInReplace(ByVal layerName As String, ByVal nIn As Integer, ByVal scheme As WeightInit) As GraphBuilder
				Return nInReplace(layerName, nIn, scheme, Nothing)
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter validateOutputLayerConfig was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function validateOutputLayerConfig(ByVal validateOutputLayerConfig_Conflict As Boolean) As GraphBuilder
				Me.validateOutputLayerConfig_Conflict = validateOutputLayerConfig_Conflict
				Return Me
			End Function

			''' <summary>
			''' Modify the architecture of a vertex layer by changing nIn of the specified layer.<br>
			''' Note that only the specified layer will be modified - all other layers will not be changed by this call.
			''' </summary>
			''' <param name="layerName"> The name of the layer to change nIn of </param>
			''' <param name="nIn">       Value of nIn to change to </param>
			''' <param name="scheme">    Weight init scheme to use for params in layerName and the layers following it </param>
			''' <returns> GraphBuilder </returns>
			Public Overridable Function nInReplace(ByVal layerName As String, ByVal nIn As Integer, ByVal scheme As WeightInit, ByVal dist As Distribution) As GraphBuilder
				Return nInReplace(layerName, nIn, scheme.getWeightInitFunction(dist))
			End Function

			''' <summary>
			''' Modify the architecture of a vertex layer by changing nIn of the specified layer.<br>
			''' Note that only the specified layer will be modified - all other layers will not be changed by this call.
			''' </summary>
			''' <param name="layerName"> The name of the layer to change nIn of </param>
			''' <param name="nIn">       Value of nIn to change to </param>
			''' <param name="scheme">    Weight init scheme to use for params in layerName and the layers following it </param>
			''' <returns> GraphBuilder </returns>
			Public Overridable Function nInReplace(ByVal layerName As String, ByVal nIn As Integer, ByVal scheme As IWeightInit) As GraphBuilder
				Preconditions.checkState(origGraph.getVertex(layerName) IsNot Nothing, "Layer with name %s not found", layerName)
				Preconditions.checkState(origGraph.getVertex(layerName).hasLayer(), "nInReplace can only be applied" & " on vertices with layers. Vertex %s does not have a layer", layerName)
				initBuilderIfReq()

				Dim layerConf As NeuralNetConfiguration = origGraph.getLayer(layerName).conf()
				Dim layerImpl As Layer = layerConf.getLayer().clone()

				Preconditions.checkState(TypeOf layerImpl Is FeedForwardLayer, "Can only use nInReplace on FeedForward layers;" & "got layer of type %s for layer name %s", layerImpl.GetType().Name, layerName)

				layerImpl.resetLayerDefaultConfig()
				Dim layerImplF As FeedForwardLayer = DirectCast(layerImpl, FeedForwardLayer)
				layerImplF.setWeightInitFn(scheme)
				layerImplF.setNIn(nIn)

				If editedVertices.Contains(layerName) AndAlso TypeOf editedConfigBuilder.getVertices().get(layerName) Is LayerVertex AndAlso nInFromNewConfig.ContainsKey(layerName) Then
					Dim l As Layer = CType(editedConfigBuilder.getVertices().get(layerName), LayerVertex).getLayerConf().getLayer()
					If TypeOf l Is FeedForwardLayer Then
						layerImplF.setNIn(nInFromNewConfig(layerName))
					End If
				End If

				editedConfigBuilder.removeVertex(layerName, False)
				Dim lv As LayerVertex = CType(origConfig.getVertices().get(layerName), LayerVertex)
				Dim lvInputs() As String = origConfig.getVertexInputs().get(layerName).toArray(New String(){})
				editedConfigBuilder.addLayer(layerName, layerImpl, lv.PreProcessor, lvInputs)
				editedVertices.Add(layerName)

				Return Me
			End Function

			Friend Overridable Function nOutReplace(ByVal layerName As String, ByVal nOut As Integer, ByVal scheme As IWeightInit, ByVal schemeNext As IWeightInit) As GraphBuilder
				initBuilderIfReq()

				If origGraph.getVertex(layerName).hasLayer() Then

					Dim layerConf As NeuralNetConfiguration = origGraph.getLayer(layerName).conf()
					Dim layerImpl As Layer = layerConf.getLayer().clone()
					layerImpl.resetLayerDefaultConfig()
					Dim layerImplF As FeedForwardLayer = DirectCast(layerImpl, FeedForwardLayer)
					layerImplF.setWeightInitFn(scheme)
					layerImplF.setNOut(nOut)

					If editedVertices.Contains(layerName) AndAlso TypeOf editedConfigBuilder.getVertices().get(layerName) Is LayerVertex AndAlso nInFromNewConfig.ContainsKey(layerName) Then
						Dim l As Layer = CType(editedConfigBuilder.getVertices().get(layerName), LayerVertex).getLayerConf().getLayer()
						If TypeOf l Is FeedForwardLayer Then
							layerImplF.setNIn(nInFromNewConfig(layerName))
						End If
					End If

					editedConfigBuilder.removeVertex(layerName, False)
					Dim lv As LayerVertex = CType(origConfig.getVertices().get(layerName), LayerVertex)
					Dim lvInputs() As String = origConfig.getVertexInputs().get(layerName).toArray(New String(){})
					editedConfigBuilder.addLayer(layerName, layerImpl, lv.PreProcessor, lvInputs)
					editedVertices.Add(layerName)

					'collect other vertices that have this vertex as inputs
					Dim fanoutVertices As IList(Of String) = New List(Of String)()
					For Each entry As KeyValuePair(Of String, IList(Of String)) In origConfig.getVertexInputs().entrySet()
						Dim currentVertex As String = entry.Key
						If Not currentVertex.Equals(layerName) Then
							If entry.Value.contains(layerName) Then
								fanoutVertices.Add(currentVertex)
							End If
						End If
					Next entry

					'change nIn of fanout
					For Each fanoutVertexName As String In fanoutVertices
						If Not origGraph.getVertex(fanoutVertexName).hasLayer() Then
							Throw New System.NotSupportedException("Cannot modify nOut of a layer vertex that feeds non-layer vertices. Use removeVertexKeepConnections followed by addVertex instead")
						End If
						layerConf = origGraph.getLayer(fanoutVertexName).conf()
						If Not (TypeOf layerConf.getLayer() Is FeedForwardLayer) Then
							Continue For
						End If
						layerImpl = layerConf.getLayer().clone()
						layerImplF = DirectCast(layerImpl, FeedForwardLayer)
						layerImplF.setWeightInitFn(schemeNext)
						layerImplF.setNIn(nOut)

						nInFromNewConfig(fanoutVertexName) = nOut

						editedConfigBuilder.removeVertex(fanoutVertexName, False)
						lv = CType(origConfig.getVertices().get(fanoutVertexName), LayerVertex)
						lvInputs = origConfig.getVertexInputs().get(fanoutVertexName).toArray(New String(){})
						editedConfigBuilder.addLayer(fanoutVertexName, layerImpl, lv.PreProcessor, lvInputs)
						editedVertices.Add(fanoutVertexName)
						If validateOutputLayerConfig_Conflict IsNot Nothing Then
							editedConfigBuilder.validateOutputLayerConfig(validateOutputLayerConfig_Conflict)
						End If
					Next fanoutVertexName
				Else
					Throw New System.ArgumentException("noutReplace can only be applied to layer vertices. " & layerName & " is not a layer vertex")
				End If
				Return Me
			End Function

			''' <summary>
			''' Remove the specified vertex from the computation graph but keep it's connections.
			''' Note the expectation here is to then add back another vertex with the same name or else the graph will be left in an invalid state
			''' Possibly with references to vertices that no longer exist </summary>
			''' <param name="outputName">
			''' @return </param>
			Public Overridable Function removeVertexKeepConnections(ByVal outputName As String) As GraphBuilder
				initBuilderIfReq()
				editedConfigBuilder.removeVertex(outputName, False)
				Return Me
			End Function

			''' <summary>
			''' Remove specified vertex and it's connections from the computation graph </summary>
			''' <param name="vertexName">
			''' @return </param>
			Public Overridable Function removeVertexAndConnections(ByVal vertexName As String) As GraphBuilder
				initBuilderIfReq()
				editedConfigBuilder.removeVertex(vertexName, True)
				Return Me
			End Function

			''' <summary>
			''' Add a layer of the specified configuration to the computation graph </summary>
			''' <param name="layerName"> </param>
			''' <param name="layer"> </param>
			''' <param name="layerInputs">
			''' @return </param>
			Public Overridable Function addLayer(ByVal layerName As String, ByVal layer As Layer, ParamArray ByVal layerInputs() As String) As GraphBuilder
				initBuilderIfReq()
				editedConfigBuilder.addLayer(layerName, layer, Nothing, layerInputs)
				editedVertices.Add(layerName)
				Return Me
			End Function

			''' <summary>
			''' Add a layer with a specified preprocessor </summary>
			''' <param name="layerName"> </param>
			''' <param name="layer"> </param>
			''' <param name="preProcessor"> </param>
			''' <param name="layerInputs">
			''' @return </param>
			Public Overridable Function addLayer(ByVal layerName As String, ByVal layer As Layer, ByVal preProcessor As InputPreProcessor, ParamArray ByVal layerInputs() As String) As GraphBuilder
				initBuilderIfReq()
				editedConfigBuilder.addLayer(layerName, layer, preProcessor, layerInputs)
				editedVertices.Add(layerName)
				Return Me
			End Function

			''' <summary>
			''' Add a vertex of the given configuration to the computation graph </summary>
			''' <param name="vertexName"> </param>
			''' <param name="vertex"> </param>
			''' <param name="vertexInputs">
			''' @return </param>
			Public Overridable Function addVertex(ByVal vertexName As String, ByVal vertex As GraphVertex, ParamArray ByVal vertexInputs() As String) As GraphBuilder
				initBuilderIfReq()
				editedConfigBuilder.addVertex(vertexName, vertex, vertexInputs)
				editedVertices.Add(vertexName)
				Return Me
			End Function

			''' <summary>
			''' Set outputs to the computation graph, will add to ones that are existing
			''' Also determines the order, like in ComputationGraphConfiguration </summary>
			''' <param name="outputNames">
			''' @return </param>
			Public Overridable Function setOutputs(ParamArray ByVal outputNames() As String) As GraphBuilder
				initBuilderIfReq()
				editedConfigBuilder.Outputs = outputNames
				Return Me
			End Function

			Friend Overridable Sub initBuilderIfReq()
				If editedConfigBuilder Is Nothing Then
					'No fine tune config has been set. One isn't required, but we need one to create the editedConfigBuilder
					'So: create an empty finetune config, which won't override anything
					'but keep the seed
					fineTuneConfiguration((New FineTuneConfiguration.Builder()).seed(origConfig.getDefaultConfiguration().getSeed()).build())
				End If
			End Sub

			''' <summary>
			''' Sets new inputs for the computation graph. This method will remove any
			''' pre-existing inputs. </summary>
			''' <param name="inputs"> String names of each graph input. </param>
			''' <returns> {@code GraphBuilder} instance. </returns>
			Public Overridable Function setInputs(ParamArray ByVal inputs() As String) As GraphBuilder
				editedConfigBuilder.setNetworkInputs(java.util.Arrays.asList(inputs))
				Return Me
			End Function

			''' <summary>
			''' Sets the input type of corresponding inputs. </summary>
			''' <param name="inputTypes"> The type of input (such as convolutional). </param>
			''' <returns> {@code GraphBuilder} instance. </returns>
			Public Overridable Function setInputTypes(ParamArray ByVal inputTypes() As InputType) As GraphBuilder
				editedConfigBuilder.InputTypes = inputTypes
				Return Me
			End Function

			Public Overridable Function addInputs(ParamArray ByVal inputNames() As String) As GraphBuilder
				editedConfigBuilder.addInputs(inputNames)
				Return Me
			End Function

			Public Overridable Function setWorkspaceMode(ByVal workspaceMode As WorkspaceMode) As GraphBuilder
				Me.workspaceMode_Conflict = workspaceMode
				Return Me
			End Function

			''' <summary>
			''' Returns a computation graph build to specifications.
			''' Init has been internally called. Can be fit directly. </summary>
			''' <returns> Computation graph </returns>
			Public Overridable Function build() As ComputationGraph
				initBuilderIfReq()

				Dim newConfig As ComputationGraphConfiguration = editedConfigBuilder.validateOutputLayerConfig(If(validateOutputLayerConfig_Conflict Is Nothing, True, validateOutputLayerConfig_Conflict)).build()
				If Me.workspaceMode_Conflict <> Nothing Then
					newConfig.setTrainingWorkspaceMode(workspaceMode_Conflict)
				End If
				Dim newGraph As New ComputationGraph(newConfig)
				newGraph.init()

				Dim topologicalOrder() As Integer = newGraph.topologicalSortOrder()
				Dim vertices() As org.deeplearning4j.nn.graph.vertex.GraphVertex = newGraph.Vertices
				If editedVertices.Count > 0 Then
					'set params from orig graph as necessary to new graph
					For i As Integer = 0 To topologicalOrder.Length - 1

						If Not vertices(topologicalOrder(i)).hasLayer() Then
							Continue For
						End If

						Dim layer As org.deeplearning4j.nn.api.Layer = vertices(topologicalOrder(i)).Layer
						Dim layerName As String = vertices(topologicalOrder(i)).VertexName
						Dim range As Long = layer.numParams()
						If range <= 0 Then
							Continue For 'some layers have no params
						End If
						If editedVertices.Contains(layerName) Then
							Continue For 'keep the changed params
						End If
						Dim origParams As INDArray = origGraph.getLayer(layerName).params()
						layer.Params = origParams.dup() 'copy over origGraph params
					Next i
				Else
					newGraph.Params = origGraph.params()
				End If

				'Freeze layers as necessary. Note: we can't simply say "everything before frozen layer X needs to be frozen
				' also" as this won't always work. For example, in1->A->C, in2->B->C, freeze B; A shouldn't be frozen, even
				' if A is before B in the topological sort order.
				'How it should be handled: use the graph structure + topological sort order.
				' If a vertex is marked to be frozen: freeze it
				' Any descendants of a frozen layer should also be frozen
				If hasFrozen Then

					'Store all frozen layers, and any vertices inheriting from said layers
					Dim allFrozen As ISet(Of String) = New HashSet(Of String)()
					Collections.addAll(allFrozen, frozenOutputAt)

					For i As Integer = topologicalOrder.Length - 1 To 0 Step -1
						Dim gv As org.deeplearning4j.nn.graph.vertex.GraphVertex = vertices(topologicalOrder(i))
						If allFrozen.Contains(gv.VertexName) Then
							If gv.hasLayer() Then
								'Need to freeze this layer - both the layer implementation, and the layer configuration
								Dim l As org.deeplearning4j.nn.api.Layer = gv.Layer
								gv.setLayerAsFrozen()

								Dim layerName As String = gv.VertexName
								Dim currLayerVertex As LayerVertex = CType(newConfig.getVertices().get(layerName), LayerVertex)
								Dim origLayerConf As Layer = currLayerVertex.getLayerConf().getLayer()
								Dim newLayerConf As Layer = New org.deeplearning4j.nn.conf.layers.misc.FrozenLayer(origLayerConf)
								newLayerConf.setLayerName(origLayerConf.LayerName)
								'Complication here(and reason for clone on next line): inner Layer (implementation)
								' NeuralNetConfiguration.layer (config) should keep the original layer config. While network
								' NNC should have the frozen layer
								Dim newNNC As NeuralNetConfiguration = currLayerVertex.getLayerConf().clone()
								currLayerVertex.setLayerConf(newNNC)
								currLayerVertex.getLayerConf().setLayer(newLayerConf)

								'Make sure the underlying layer doesn't change:
								Dim vars As IList(Of String) = currLayerVertex.getLayerConf().variables(True)
								currLayerVertex.getLayerConf().clearVariables()
								For Each s As String In vars
									newNNC.variables(False).Add(s)
								Next s

								'We also need to place the layer in the CompGraph Layer[] (replacing the old one)
								'This could no doubt be done more efficiently
								Dim layers() As org.deeplearning4j.nn.api.Layer = newGraph.Layers
								For j As Integer = 0 To layers.Length - 1
									If layers(j) Is l Then
										layers(j) = gv.Layer 'Place the new frozen layer to replace the original layer
										Exit For
									End If
								Next j
							Else
								If Not (TypeOf gv Is InputVertex) Then
									Dim currVertexConf As GraphVertex = newConfig.getVertices().get(gv.VertexName)
									Dim newVertexConf As GraphVertex = New org.deeplearning4j.nn.conf.graph.FrozenVertex(currVertexConf)
									newConfig.getVertices().put(gv.VertexName, newVertexConf)
									vertices(topologicalOrder(i)) = New FrozenVertex(gv)
								End If
							End If

							'Also: mark any inputs as to be frozen also
							Dim inputs() As VertexIndices = gv.InputVertices
							If inputs IsNot Nothing AndAlso inputs.Length > 0 Then
								For j As Integer = 0 To inputs.Length - 1
									Dim inputVertexIdx As Integer = inputs(j).VertexIndex
									Dim alsoFreeze As String = vertices(inputVertexIdx).VertexName
									allFrozen.Add(alsoFreeze)
								Next j
							End If
						End If
					Next i
					newGraph.initGradientsView()
				End If
				Return newGraph
			End Function
		End Class
	End Class

End Namespace