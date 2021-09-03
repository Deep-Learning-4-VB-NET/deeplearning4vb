Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Model = org.deeplearning4j.nn.api.Model
Imports Trainable = org.deeplearning4j.nn.api.Trainable
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports BaseLayer = org.deeplearning4j.nn.conf.layers.BaseLayer
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports GraphVertex = org.deeplearning4j.nn.graph.vertex.GraphVertex
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports MultiLayerUpdater = org.deeplearning4j.nn.updater.MultiLayerUpdater
Imports UpdaterBlock = org.deeplearning4j.nn.updater.UpdaterBlock
Imports ComputationGraphUpdater = org.deeplearning4j.nn.updater.graph.ComputationGraphUpdater
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports ISchedule = org.nd4j.linalg.schedule.ISchedule

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

Namespace org.deeplearning4j.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class NetworkUtils
	Public Class NetworkUtils

		Private Sub New()
		End Sub

		''' <summary>
		''' Convert a MultiLayerNetwork to a ComputationGraph
		''' </summary>
		''' <returns> ComputationGraph equivalent to this network (including parameters and updater state) </returns>
		Public Shared Function toComputationGraph(ByVal net As MultiLayerNetwork) As ComputationGraph

			'We rely heavily here on the fact that the topological sort order - and hence the layout of parameters - is
			' by definition the identical for a MLN and "single stack" computation graph. This also has to hold
			' for the updater state...

			Dim b As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).dataType(net.LayerWiseConfigurations.getDataType()).graphBuilder()

			Dim origConf As MultiLayerConfiguration = net.LayerWiseConfigurations.clone()


			Dim layerIdx As Integer = 0
			Dim lastLayer As String = "in"
			b.addInputs("in")
			For Each c As NeuralNetConfiguration In origConf.getConfs()
				Dim currLayer As String = layerIdx.ToString()

				Dim preproc As InputPreProcessor = origConf.getInputPreProcess(layerIdx)
				b.addLayer(currLayer, c.getLayer(), preproc, lastLayer)

				lastLayer = currLayer
				layerIdx += 1
			Next c
			b.Outputs = lastLayer

			Dim conf As ComputationGraphConfiguration = b.build()

			Dim cg As New ComputationGraph(conf)
			cg.init()

			cg.Params = net.params()

			'Also copy across updater state:
			Dim updaterState As INDArray = net.Updater.StateViewArray
			If updaterState IsNot Nothing Then
				cg.Updater.getUpdaterStateViewArray().assign(updaterState)
			End If

			Return cg
		End Function

		''' <summary>
		''' Set the learning rate for all layers in the network to the specified value. Note that if any learning rate
		''' schedules are currently present, these will be removed in favor of the new (fixed) learning rate.<br>
		''' <br>
		''' <b>Note</b>: <i>This method not free from a performance point of view</i>: a proper learning rate schedule
		''' should be used in preference to calling this method at every iteration.
		''' </summary>
		''' <param name="net">   Network to set the LR for </param>
		''' <param name="newLr"> New learning rate for all layers </param>
		Public Shared Sub setLearningRate(ByVal net As MultiLayerNetwork, ByVal newLr As Double)
			setLearningRate(net, newLr, Nothing)
		End Sub

		Private Shared Sub setLearningRate(ByVal net As MultiLayerNetwork, ByVal newLr As Double, ByVal lrSchedule As ISchedule)
			Dim nLayers As Integer = net.getnLayers()
			For i As Integer = 0 To nLayers - 1
				setLearningRate(net, i, newLr, lrSchedule, False)
			Next i
			refreshUpdater(net)
		End Sub

		Private Shared Sub setLearningRate(ByVal net As MultiLayerNetwork, ByVal layerNumber As Integer, ByVal newLr As Double, ByVal newLrSchedule As ISchedule, ByVal refreshUpdater As Boolean)

			Dim l As Layer = net.getLayer(layerNumber).conf().getLayer()
			If TypeOf l Is BaseLayer Then
				Dim bl As BaseLayer = DirectCast(l, BaseLayer)
				Dim u As IUpdater = bl.getIUpdater()
				If u IsNot Nothing AndAlso u.hasLearningRate() Then
					If newLrSchedule IsNot Nothing Then
						u.setLrAndSchedule(Double.NaN, newLrSchedule)
					Else
						u.setLrAndSchedule(newLr, Nothing)
					End If
				End If

				'Need to refresh the updater - if we change the LR (or schedule) we may rebuild the updater blocks, which are
				' built by creating blocks of params with the same configuration
				If refreshUpdater Then
					NetworkUtils.refreshUpdater(net)
				End If
			End If
		End Sub

		Private Shared Sub refreshUpdater(ByVal net As MultiLayerNetwork)
			Dim origUpdaterState As INDArray = net.Updater.StateViewArray
			Dim origUpdater As MultiLayerUpdater = DirectCast(net.Updater, MultiLayerUpdater)
			net.Updater = Nothing
			Dim newUpdater As MultiLayerUpdater = DirectCast(net.Updater, MultiLayerUpdater)
			Dim newUpdaterState As INDArray = rebuildUpdaterStateArray(origUpdaterState, origUpdater.getUpdaterBlocks(), newUpdater.getUpdaterBlocks())
			newUpdater.StateViewArray = newUpdaterState
		End Sub

		''' <summary>
		''' Set the learning rate schedule for all layers in the network to the specified schedule.
		''' This schedule will replace any/all existing schedules, and also any fixed learning rate values.<br>
		''' Note that the iteration/epoch counts will <i>not</i> be reset. Use <seealso cref="MultiLayerConfiguration.setIterationCount(Integer)"/>
		''' and <seealso cref="MultiLayerConfiguration.setEpochCount(Integer)"/> if this is required
		''' </summary>
		''' <param name="newLrSchedule"> New learning rate schedule for all layers </param>
		Public Shared Sub setLearningRate(ByVal net As MultiLayerNetwork, ByVal newLrSchedule As ISchedule)
			setLearningRate(net, Double.NaN, newLrSchedule)
		End Sub

		''' <summary>
		''' Set the learning rate for a single layer in the network to the specified value. Note that if any learning rate
		''' schedules are currently present, these will be removed in favor of the new (fixed) learning rate.<br>
		''' <br>
		''' <b>Note</b>: <i>This method not free from a performance point of view</i>: a proper learning rate schedule
		''' should be used in preference to calling this method at every iteration. Note also that
		''' <seealso cref="setLearningRate(MultiLayerNetwork, Double)"/> should also be used in preference, when all layers need to be set to a new LR
		''' </summary>
		''' <param name="layerNumber"> Number of the layer to set the LR for </param>
		''' <param name="newLr">       New learning rate for a single layers </param>
		Public Shared Sub setLearningRate(ByVal net As MultiLayerNetwork, ByVal layerNumber As Integer, ByVal newLr As Double)
			setLearningRate(net, layerNumber, newLr, Nothing, True)
		End Sub

		''' <summary>
		''' Set the learning rate schedule for a single layer in the network to the specified value.<br>
		''' Note also that <seealso cref="setLearningRate(MultiLayerNetwork, ISchedule)"/> should also be used in preference, when all layers need
		''' to be set to a new LR schedule.<br>
		''' This schedule will replace any/all existing schedules, and also any fixed learning rate values.<br>
		''' Note also that the iteration/epoch counts will <i>not</i> be reset. Use <seealso cref="MultiLayerConfiguration.setIterationCount(Integer)"/>
		''' and <seealso cref="MultiLayerConfiguration.setEpochCount(Integer)"/> if this is required
		''' </summary>
		''' <param name="layerNumber"> Number of the layer to set the LR schedule for </param>
		''' <param name="lrSchedule">  New learning rate for a single layer </param>
		Public Shared Sub setLearningRate(ByVal net As MultiLayerNetwork, ByVal layerNumber As Integer, ByVal lrSchedule As ISchedule)
			setLearningRate(net, layerNumber, Double.NaN, lrSchedule, True)
		End Sub

		''' <summary>
		''' Get the current learning rate, for the specified layer, fromthe network.
		''' Note: If the layer has no learning rate (no parameters, or an updater without a learning rate) then null is returned
		''' </summary>
		''' <param name="net">         Network </param>
		''' <param name="layerNumber"> Layer number to get the learning rate for </param>
		''' <returns> Learning rate for the specified layer, or null </returns>
		Public Shared Function getLearningRate(ByVal net As MultiLayerNetwork, ByVal layerNumber As Integer) As Double?
			Dim l As Layer = net.getLayer(layerNumber).conf().getLayer()
			Dim iter As Integer = net.IterationCount
			Dim epoch As Integer = net.EpochCount
			If TypeOf l Is BaseLayer Then
				Dim bl As BaseLayer = DirectCast(l, BaseLayer)
				Dim u As IUpdater = bl.getIUpdater()
				If u IsNot Nothing AndAlso u.hasLearningRate() Then
					Dim d As Double = u.getLearningRate(iter, epoch)
					If Double.IsNaN(d) Then
						Return Nothing
					End If
					Return d
				End If
				Return Nothing
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Set the learning rate for all layers in the network to the specified value. Note that if any learning rate
		''' schedules are currently present, these will be removed in favor of the new (fixed) learning rate.<br>
		''' <br>
		''' <b>Note</b>: <i>This method not free from a performance point of view</i>: a proper learning rate schedule
		''' should be used in preference to calling this method at every iteration.
		''' </summary>
		''' <param name="net">   Network to set the LR for </param>
		''' <param name="newLr"> New learning rate for all layers </param>
		Public Shared Sub setLearningRate(ByVal net As ComputationGraph, ByVal newLr As Double)
			setLearningRate(net, newLr, Nothing)
		End Sub

		Private Shared Sub setLearningRate(ByVal net As ComputationGraph, ByVal newLr As Double, ByVal lrSchedule As ISchedule)
			Dim layers() As org.deeplearning4j.nn.api.Layer = net.Layers
			For i As Integer = 0 To layers.Length - 1
				setLearningRate(net, layers(i).conf().getLayer().getLayerName(), newLr, lrSchedule, False)
			Next i
			refreshUpdater(net)
		End Sub

		Private Shared Sub setLearningRate(ByVal net As ComputationGraph, ByVal layerName As String, ByVal newLr As Double, ByVal newLrSchedule As ISchedule, ByVal refreshUpdater As Boolean)

			Dim l As Layer = net.getLayer(layerName).conf().getLayer()
			If TypeOf l Is BaseLayer Then
				Dim bl As BaseLayer = DirectCast(l, BaseLayer)
				Dim u As IUpdater = bl.getIUpdater()
				If u IsNot Nothing AndAlso u.hasLearningRate() Then
					If newLrSchedule IsNot Nothing Then
						u.setLrAndSchedule(Double.NaN, newLrSchedule)
					Else
						u.setLrAndSchedule(newLr, Nothing)
					End If
				End If

				'Need to refresh the updater - if we change the LR (or schedule) we may rebuild the updater blocks, which are
				' built by creating blocks of params with the same configuration
				If refreshUpdater Then
					NetworkUtils.refreshUpdater(net)
				End If
			End If
		End Sub

		Private Shared Sub refreshUpdater(ByVal net As ComputationGraph)
			Dim origUpdaterState As INDArray = net.Updater.StateViewArray
			Dim uOrig As ComputationGraphUpdater = net.Updater
			net.Updater = Nothing
			Dim uNew As ComputationGraphUpdater = net.Updater
			Dim newUpdaterState As INDArray = rebuildUpdaterStateArray(origUpdaterState, uOrig.getUpdaterBlocks(), uNew.getUpdaterBlocks())
			uNew.StateViewArray = newUpdaterState
		End Sub

		''' <summary>
		''' Set the learning rate schedule for all layers in the network to the specified schedule.
		''' This schedule will replace any/all existing schedules, and also any fixed learning rate values.<br>
		''' Note that the iteration/epoch counts will <i>not</i> be reset. Use <seealso cref="ComputationGraphConfiguration.setIterationCount(Integer)"/>
		''' and <seealso cref="ComputationGraphConfiguration.setEpochCount(Integer)"/> if this is required
		''' </summary>
		''' <param name="newLrSchedule"> New learning rate schedule for all layers </param>
		Public Shared Sub setLearningRate(ByVal net As ComputationGraph, ByVal newLrSchedule As ISchedule)
			setLearningRate(net, Double.NaN, newLrSchedule)
		End Sub

		''' <summary>
		''' Set the learning rate for a single layer in the network to the specified value. Note that if any learning rate
		''' schedules are currently present, these will be removed in favor of the new (fixed) learning rate.<br>
		''' <br>
		''' <b>Note</b>: <i>This method not free from a performance point of view</i>: a proper learning rate schedule
		''' should be used in preference to calling this method at every iteration. Note also that
		''' <seealso cref="setLearningRate(ComputationGraph, Double)"/> should also be used in preference, when all layers need to be set to a new LR
		''' </summary>
		''' <param name="layerName"> Name of the layer to set the LR for </param>
		''' <param name="newLr">     New learning rate for a single layers </param>
		Public Shared Sub setLearningRate(ByVal net As ComputationGraph, ByVal layerName As String, ByVal newLr As Double)
			setLearningRate(net, layerName, newLr, Nothing, True)
		End Sub

		''' <summary>
		''' Set the learning rate schedule for a single layer in the network to the specified value.<br>
		''' Note also that <seealso cref="setLearningRate(ComputationGraph, ISchedule)"/> should also be used in preference, when all
		''' layers need to be set to a new LR schedule.<br>
		''' This schedule will replace any/all existing schedules, and also any fixed learning rate values.<br>
		''' Note also that the iteration/epoch counts will <i>not</i> be reset. Use <seealso cref="ComputationGraphConfiguration.setIterationCount(Integer)"/>
		''' and <seealso cref="ComputationGraphConfiguration.setEpochCount(Integer)"/> if this is required
		''' </summary>
		''' <param name="layerName">  Name of the layer to set the LR schedule for </param>
		''' <param name="lrSchedule"> New learning rate for a single layer </param>
		Public Shared Sub setLearningRate(ByVal net As ComputationGraph, ByVal layerName As String, ByVal lrSchedule As ISchedule)
			setLearningRate(net, layerName, Double.NaN, lrSchedule, True)
		End Sub

		''' <summary>
		''' Get the current learning rate, for the specified layer, from the network.
		''' Note: If the layer has no learning rate (no parameters, or an updater without a learning rate) then null is returned
		''' </summary>
		''' <param name="net">       Network </param>
		''' <param name="layerName"> Layer name to get the learning rate for </param>
		''' <returns> Learning rate for the specified layer, or null </returns>
		Public Shared Function getLearningRate(ByVal net As ComputationGraph, ByVal layerName As String) As Double?
			Dim l As Layer = net.getLayer(layerName).conf().getLayer()
			Dim iter As Integer = net.Configuration.getIterationCount()
			Dim epoch As Integer = net.Configuration.getEpochCount()
			If TypeOf l Is BaseLayer Then
				Dim bl As BaseLayer = DirectCast(l, BaseLayer)
				Dim u As IUpdater = bl.getIUpdater()
				If u IsNot Nothing AndAlso u.hasLearningRate() Then
					Dim d As Double = u.getLearningRate(iter, epoch)
					If Double.IsNaN(d) Then
						Return Nothing
					End If
					Return d
				End If
				Return Nothing
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Currently supports <seealso cref="MultiLayerNetwork"/> and <seealso cref="ComputationGraph"/> models.
		''' Pull requests to support additional <code>org.deeplearning4j</code> models are welcome.
		''' </summary>
		''' <param name="model"> Model to use </param>
		''' <param name="input"> Inputs to the model </param>
		''' <returns> output Outputs of the model </returns>
		''' <seealso cref= ComputationGraph#outputSingle(INDArray...) </seealso>
		''' <seealso cref= MultiLayerNetwork#output(INDArray) </seealso>
		Public Shared Function output(ByVal model As Model, ByVal input As INDArray) As INDArray

			If TypeOf model Is MultiLayerNetwork Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.multilayer.MultiLayerNetwork multiLayerNetwork = (org.deeplearning4j.nn.multilayer.MultiLayerNetwork) model;
				Dim multiLayerNetwork As MultiLayerNetwork = DirectCast(model, MultiLayerNetwork)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray output = multiLayerNetwork.output(input);
'JAVA TO VB CONVERTER NOTE: The local variable output was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
				Dim output_Conflict As INDArray = multiLayerNetwork.output(input)
				Return output_Conflict
			End If

			If TypeOf model Is ComputationGraph Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.graph.ComputationGraph computationGraph = (org.deeplearning4j.nn.graph.ComputationGraph) model;
				Dim computationGraph As ComputationGraph = DirectCast(model, ComputationGraph)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray output = computationGraph.outputSingle(input);
'JAVA TO VB CONVERTER NOTE: The local variable output was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
				Dim output_Conflict As INDArray = computationGraph.outputSingle(input)
				Return output_Conflict
			End If

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String message;
			Dim message As String
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			If model.GetType().FullName.StartsWith("org.deeplearning4j", StringComparison.Ordinal) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				message = model.GetType().FullName & " models are not yet supported and " & "pull requests are welcome: https://github.com/eclipse/deeplearning4j"
			Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				message = model.GetType().FullName & " models are unsupported."
			End If

			Throw New System.NotSupportedException(message)
		End Function

		''' <summary>
		''' Remove any instances of the specified type from the list.
		''' This includes any subtypes. </summary>
		''' <param name="list">   List. May be null </param>
		''' <param name="remove"> Type of objects to remove </param>
		Public Shared Sub removeInstances(Of T1)(ByVal list As IList(Of T1), ByVal remove As Type)
			removeInstancesWithWarning(list, remove, Nothing)
		End Sub

		Public Shared Sub removeInstancesWithWarning(Of T1)(ByVal list As IList(Of T1), ByVal remove As Type, ByVal warning As String)
			If list Is Nothing OrElse list.Count = 0 Then
				Return
			End If
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: Iterator<?> iter = list.iterator();
			Dim iter As IEnumerator(Of Object) = list.GetEnumerator()
			Do While iter.MoveNext()
				Dim o As Object = iter.Current
				If remove.IsAssignableFrom(o.GetType()) Then
					If warning IsNot Nothing Then
						log.warn(warning)
					End If
'JAVA TO VB CONVERTER TODO TASK: .NET enumerators are read-only:
					iter.remove()
				End If
			Loop
		End Sub


		''' <summary>
		''' Rebuild the updater state after a learning rate change.
		''' With updaters like Adam, they have 2 components... m and v array, for a total updater state size of 2*numParams.
		''' Because we combine across parameters and layers where possible (smaller number of larger operations -> more efficient)
		''' we can sometimes need to rearrange the updater state array.
		''' For example, if the original updater state for Adam is organized like [mParam1, mParam2, vParam1, vParam2] in one block
		''' and we change the learning rate for one of the layers, param 1 and param2 now belong to different updater blocks.
		''' Consequently, we need to rearrange the updater state to be like [mParam1][vParam1] in block 1, [mParam2][vParam2] in block 2
		''' </summary>
		''' <param name="origUpdaterState"> Original updater state view array </param>
		''' <param name="orig">             Original updater blocks </param>
		''' <param name="newUpdater">       New updater blocks </param>
		''' <returns> New state view array </returns>
		Protected Friend Shared Function rebuildUpdaterStateArray(ByVal origUpdaterState As INDArray, ByVal orig As IList(Of UpdaterBlock), ByVal newUpdater As IList(Of UpdaterBlock)) As INDArray
			If origUpdaterState Is Nothing Then
				Return origUpdaterState
			End If

			'First: check if there has been any change in the updater blocks to warrant rearranging the updater state view array
			If orig.Count = newUpdater.Count Then
				Dim allEq As Boolean = True
				For i As Integer = 0 To orig.Count - 1
					Dim ub1 As UpdaterBlock = orig(i)
					Dim ub2 As UpdaterBlock = newUpdater(i)
					If Not ub1.getLayersAndVariablesInBlock().Equals(ub2.getLayersAndVariablesInBlock()) Then
						allEq = False
						Exit For
					End If
				Next i
				If allEq Then
					Return origUpdaterState
				End If
			End If

			Dim stateViewsPerParam As IDictionary(Of String, IList(Of INDArray)) = New Dictionary(Of String, IList(Of INDArray))()
			For Each ub As UpdaterBlock In orig
				Dim params As IList(Of UpdaterBlock.ParamState) = ub.getLayersAndVariablesInBlock()
				Dim blockPStart As Integer = ub.getParamOffsetStart()
				Dim blockPEnd As Integer = ub.getParamOffsetEnd()

				Dim blockUStart As Integer = ub.getUpdaterViewOffsetStart()
				Dim blockUEnd As Integer = ub.getUpdaterViewOffsetEnd()

				Dim paramsMultiplier As Integer = (blockUEnd-blockUStart)\(blockPEnd-blockPStart) 'Updater state length should be exactly 0, 1, 2 or 3x number of params

				Dim updaterView As INDArray = ub.getUpdaterView()
				Dim nParamsInBlock As Long = blockPEnd - blockPStart

				Dim soFar As Long = 0
				For [sub] As Integer = 0 To paramsMultiplier - 1
					'subsetUpdaterView: [m0, m1, m2] etc
					Dim subsetUpdaterView As INDArray = updaterView.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(soFar, soFar + nParamsInBlock))

					Dim offsetWithinSub As Long = 0
					For Each ps As UpdaterBlock.ParamState In params
						Dim idx As Integer = getId(ps.getLayer())
						Dim paramName As String = idx & "_" & ps.getParamName()
						Dim pv As INDArray = ps.getParamView()
						Dim nParamsThisParam As Long = pv.length()

						Dim currSplit As INDArray = subsetUpdaterView.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(offsetWithinSub, offsetWithinSub + nParamsThisParam))
						If Not stateViewsPerParam.ContainsKey(paramName) Then
							stateViewsPerParam(paramName) = New List(Of INDArray)()
						End If
						stateViewsPerParam(paramName).Add(currSplit)
						offsetWithinSub += nParamsThisParam
					Next ps

					soFar += nParamsInBlock
				Next [sub]
			Next ub

			'Now that we've got updater state per param, we need to reconstruct it in an order suitable for the new updater blocks...
			Dim toConcat As IList(Of INDArray) = New List(Of INDArray)()
			For Each ub As UpdaterBlock In newUpdater
				Dim ps As IList(Of UpdaterBlock.ParamState) = ub.getLayersAndVariablesInBlock()
				Dim idx As Integer = getId(ps(0).getLayer())
				Dim firstParam As String = idx & "_" & ps(0).getParamName()
				Dim size As Integer = stateViewsPerParam(firstParam).Count
				'For multiple params in the one block, we want to order like [a0, b0, c0][a1,b1,c1]
				For i As Integer = 0 To size - 1
					For Each p As UpdaterBlock.ParamState In ps
						idx = getId(p.getLayer())
						Dim paramName As String = idx & "_" & p.getParamName()
						Dim arr As INDArray = stateViewsPerParam(paramName)(i)
						toConcat.Add(arr)
					Next p
				Next i
			Next ub
			Dim newUpdaterState As INDArray = Nd4j.hstack(toConcat)
			Preconditions.checkState(newUpdaterState.rank() = 2, "Expected rank 2")
			Preconditions.checkState(origUpdaterState.length() = newUpdaterState.length(), "Updater state array lengths should be equal: got %s s. %s", origUpdaterState.length(), newUpdaterState.length())
			Return newUpdaterState
		End Function


		Private Shared Function getId(ByVal trainable As Trainable) As Integer
			If TypeOf trainable Is GraphVertex Then
				Dim gv As GraphVertex = DirectCast(trainable, GraphVertex)
				Return gv.VertexIndex
			Else
				Dim l As org.deeplearning4j.nn.api.Layer = DirectCast(trainable, org.deeplearning4j.nn.api.Layer)
				Return l.Index
			End If
		End Function

	End Class

End Namespace