Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Model = org.deeplearning4j.nn.api.Model
Imports Trainable = org.deeplearning4j.nn.api.Trainable
Imports Updater = org.deeplearning4j.nn.api.Updater
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Norm2 = org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater

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

Namespace org.deeplearning4j.nn.updater


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter public abstract class BaseMultiLayerUpdater<T extends org.deeplearning4j.nn.api.Model> implements org.deeplearning4j.nn.api.Updater
	<Serializable>
	Public MustInherit Class BaseMultiLayerUpdater(Of T As org.deeplearning4j.nn.api.Model)
		Implements Updater

		Protected Friend ReadOnly network As T
		Protected Friend layersByName As IDictionary(Of String, Trainable)
		Protected Friend ReadOnly updaterBlocks As IList(Of UpdaterBlock)
		Protected Friend updaterStateViewArray As INDArray
		Protected Friend initializedMinibatchDivision As Boolean
		Protected Friend gradientsForMinibatchDivision As IList(Of INDArray)

		Public Sub New(ByVal network As T)
			Me.New(network, Nothing)
		End Sub

		''' 
		''' <param name="network">      Network to create the updater for </param>
		''' <param name="updaterState"> The updater state to use. Note: This array is used *directly* and isn't copied/cloned </param>
		Public Sub New(ByVal network As T, ByVal updaterState As INDArray)
			Me.network = network
			Dim layers() As Trainable = OrderedLayers 'May also include vertices

			Dim updaterStateSize As Integer = 0
			'Iterate through layers, and variables for each layer.
			'While the updater configuration is the same: combine into one op, rather than doing a lot of smaller
			' (yet identical) ops.
			Dim lastLayer As Trainable = Nothing
			Dim lastVariable As String = Nothing
			Dim currentBlock As UpdaterBlock = Nothing
			updaterBlocks = New List(Of UpdaterBlock)()


			Dim paramsView As INDArray = network.params()
			Dim gradientView As INDArray = FlattenedGradientsView
			Dim paramsViewSoFar As Integer = 0
			Dim currentUpdaterOffset As Integer = 0
			For i As Integer = 0 To layers.Length - 1
				Dim layerParamTable As IDictionary(Of String, INDArray) = layers(i).paramTable(False)
				If layerParamTable IsNot Nothing Then
					Dim variables As IList(Of String) = New List(Of String)(layerParamTable.Keys) 'Is from a set, but iteration order should be fixed per layer as it's a from a LinkedHashSet
					For j As Integer = 0 To variables.Count - 1
						Dim var As String = variables(j)
						Dim paramSizeThisVariable As Long = layerParamTable(var).length()
						Dim u As IUpdater = layers(i).Config.getUpdaterByParam(var)
						Preconditions.checkNotNull(u, "Updater for parameter %s, layer ""%s"" was null", var, layers(i).Config.LayerName)
						Dim updaterStateSizeThisVariable As Integer = CInt(u.stateSize(paramSizeThisVariable))

						Dim gradientViewSubset As INDArray = Nothing
						Dim paramsViewSubset As INDArray = Nothing
						If paramSizeThisVariable > 0 Then
							paramsViewSubset = paramsView.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(paramsViewSoFar, paramsViewSoFar + paramSizeThisVariable))
							gradientViewSubset = gradientView.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(paramsViewSoFar, paramsViewSoFar + paramSizeThisVariable))
						End If

						'First: decide whether to add to the existing updater block, or create a new one
						If currentBlock Is Nothing OrElse Not UpdaterUtils.updaterConfigurationsEquals(lastLayer, lastVariable, layers(i), var) Then

							If paramsViewSoFar + paramSizeThisVariable > Integer.MaxValue OrElse paramsViewSoFar + paramSizeThisVariable > Integer.MaxValue Then
								Throw New ND4JArraySizeException()
							End If
							'Create a new block
							Dim list As IList(Of UpdaterBlock.ParamState) = New List(Of UpdaterBlock.ParamState)()
							list.Add(New UpdaterBlock.ParamState(layers(i), var, paramsViewSoFar, CInt(paramsViewSoFar + paramSizeThisVariable), paramsViewSubset, gradientViewSubset))
							currentBlock = New UpdaterBlock(paramsViewSoFar, CInt(paramsViewSoFar + paramSizeThisVariable), currentUpdaterOffset, currentUpdaterOffset + updaterStateSizeThisVariable, list)

							updaterBlocks.Add(currentBlock)
						Else
							Dim newOffset As Long = currentBlock.getParamOffsetEnd() + paramSizeThisVariable
							If newOffset > Integer.MaxValue Then
								Throw New ND4JArraySizeException()
							End If
							'Add to existing updater block
							currentBlock.setParamOffsetEnd(CInt(newOffset))
							currentBlock.setUpdaterViewOffsetEnd(currentBlock.getUpdaterViewOffsetEnd() + updaterStateSizeThisVariable)
							currentBlock.getLayersAndVariablesInBlock().add(New UpdaterBlock.ParamState(layers(i), var, paramsViewSoFar, CInt(paramsViewSoFar + paramSizeThisVariable), paramsViewSubset, gradientViewSubset))
						End If

						lastLayer = layers(i)
						lastVariable = variables(j)
						updaterStateSize += updaterStateSizeThisVariable
						paramsViewSoFar += paramSizeThisVariable
						currentUpdaterOffset += updaterStateSizeThisVariable
					Next j
				End If
			Next i

			'Initialize the updater state, if required
			Dim updaterRequiresInit As Boolean = False
			If updaterState IsNot Nothing Then
				updaterStateViewArray = updaterState
				updaterRequiresInit = False
			ElseIf updaterStateSize > 0 Then
				'May be 0 if all SGD or NONE updaters, for example
				updaterStateViewArray = Nd4j.createUninitialized(network.params().dataType(), New Long() {1, updaterStateSize}, Nd4j.order())
				updaterRequiresInit = True
			End If

			'Create and set up the updaters, for the updater blocks:
			Dim updaterViewSoFar As Integer = 0
			paramsViewSoFar = 0
			For i As Integer = 0 To updaterBlocks.Count - 1
				Dim ub As UpdaterBlock = updaterBlocks(i)

				Dim viewStateSize As Integer = ub.getUpdaterViewOffsetEnd() - ub.getUpdaterViewOffsetStart()
				Dim gradSize As Integer = ub.getParamOffsetEnd() - ub.getParamOffsetStart()

				If viewStateSize > 0 Then
					Dim updaterViewSubset As INDArray = updaterStateViewArray.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(updaterViewSoFar, updaterViewSoFar + viewStateSize))
					ub.setUpdaterView(updaterViewSubset)
					ub.setUpdaterViewRequiresInitialization(updaterRequiresInit)
				End If

				If gradSize > 0 Then
					Dim gradientViewSubset As INDArray = gradientView.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(paramsViewSoFar, paramsViewSoFar + gradSize))
					ub.setGradientView(gradientViewSubset)
				End If

				ub.init()

				updaterViewSoFar += viewStateSize
				paramsViewSoFar += gradSize
			Next i
		End Sub

		''' 
		''' <returns> Array of layers, in the correct order (i.e., same order as the parameter/gradient/updater flattening
		''' order - input to output for MultiLayerNetwork, or topological order for ComputationGraph) </returns>
		Protected Friend MustOverride ReadOnly Property OrderedLayers As Trainable()

		''' <returns> The flattened gradient view array for the model </returns>
		Protected Friend MustOverride ReadOnly Property FlattenedGradientsView As INDArray

		''' <returns> The flattened parameter array for the model </returns>
		Protected Friend MustOverride ReadOnly Property Params As INDArray

		''' <returns> True if the configuration for the model is set to minibatch (divide by minibatch size), false otherwise </returns>
		Protected Friend MustOverride ReadOnly Property MiniBatch As Boolean

		''' <summary>
		''' Set the view array. Note that this does an assign operation - the provided array is not stored internally.
		''' </summary>
		''' <param name="viewArray"> The new updater state </param>
		Public Overridable Property StateViewArray As INDArray
			Set(ByVal viewArray As INDArray)
				If Me.updaterStateViewArray Is Nothing Then
					If viewArray Is Nothing Then
						Return 'No op - for example, SGD and NoOp updater - i.e., no stored state
					Else
						Throw New System.InvalidOperationException("Attempting to set updater state view array with null value")
					End If
				End If
				If Me.updaterStateViewArray.length() <> viewArray.length() Then
					Throw New System.InvalidOperationException("Invalid input: view arrays differ in length. " & "Expected length " & Me.updaterStateViewArray.length() & ", got length " & viewArray.length())
				End If
				Me.updaterStateViewArray.assign(viewArray)
			End Set
			Get
				Return updaterStateViewArray
			End Get
		End Property

		Public Overridable Sub setStateViewArray(ByVal layer As Trainable, ByVal viewArray As INDArray, ByVal initialize As Boolean)
			Me.StateViewArray = viewArray
		End Sub


		''' <summary>
		''' A synchronized version of <seealso cref="getStateViewArray()"/> that duplicates the view array internally.
		''' This should be used in preference to <seealso cref="getStateViewArray()"/> when the updater state is accessed in one
		''' thread while another thread is using the updater for training. </summary>
		''' <returns> A copy (duplicate) of the updater state </returns>
		Public Overridable ReadOnly Property StateViewArrayCopy As INDArray
			Get
				SyncLock Me
					Nd4j.Executioner.commit()
					Return updaterStateViewArray.dup()
				End SyncLock
			End Get
		End Property

		Public Overridable Sub update(ByVal layer As Trainable, ByVal gradient As Gradient, ByVal iteration As Integer, ByVal epoch As Integer, ByVal batchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr)
			update(gradient, iteration, epoch, batchSize, workspaceMgr)
		End Sub

		''' <summary>
		''' Update the gradient for the model.
		''' This operates in 3 steps:
		''' 1. Pre-apply: gradient clipping, etc on a per-layer basis
		''' 2. Execute the updater (Adam, Nesterov momentum, etc) - in blocks of layers at a time
		''' 3. Divide by minibatch size
		''' </summary>
		''' <param name="gradient">  Gradient to updater </param>
		''' <param name="iteration"> The current iteration (i.e., number of parameter updates so far) </param>
		''' <param name="batchSize"> The current minibatch size (number of examples) </param>
		Public Overridable Sub update(ByVal gradient As Gradient, ByVal iteration As Integer, ByVal epoch As Integer, ByVal batchSize As Integer, ByVal workspaceMgr As LayerWorkspaceMgr)
			SyncLock Me
        
				'First: check if gradient is standard or external...
				'In a MultiLayerNetwork, the INDArray returned by .gradient() is always the standard full view array
				' hence should be the same object under normal circumstances
				Dim isExternal As Boolean = gradient.gradient() IsNot FlattenedGradientsView
        
				'Split up the gradients on a per-layer basis, for pre-apply
				Dim layerGradients As IDictionary(Of String, Gradient) = New Dictionary(Of String, Gradient)()
        
				Dim layers() As Trainable = OrderedLayers
				If layers.Length = 1 AndAlso SingleLayerUpdater Then
					layerGradients(layers(0).Config.LayerName) = gradient
				Else
					For Each gradientPair As KeyValuePair(Of String, INDArray) In gradient.gradientForVariable().SetOfKeyValuePairs()
						Dim key As String = gradientPair.Key
						Dim idx As Integer = key.LastIndexOf("_"c)
						If idx = -1 Then
							Throw New System.InvalidOperationException("Invalid key: Gradient key does not have layer separator: """ & key & """")
						End If
						Dim layerName As String = key.Substring(0, idx)
        
						Dim g As Gradient = layerGradients(layerName)
						If g Is Nothing Then
							g = New DefaultGradient()
							layerGradients(layerName) = g
						End If
        
						Dim newKey As String = key.Substring(idx + 1)
						g.setGradientFor(newKey, gradientPair.Value)
					Next gradientPair
				End If
        
				If MiniBatch Then
					divideByMinibatch(isExternal, gradient, batchSize)
				End If
        
				'PRE apply (gradient clipping, etc): done on a per-layer basis
				For Each entry As KeyValuePair(Of String, Gradient) In layerGradients.SetOfKeyValuePairs()
					Dim layerName As String = entry.Key
					Dim layer As Trainable = layersByName(layerName)
        
					preApply(layer, layerGradients(layerName), iteration)
				Next entry
        
				'Apply the updaters in blocks. This also applies LR and momentum schedules, L1 and L2
				If Me.GetType() <> GetType(LayerUpdater) Then
					'OK for LayerUpdater as this is part of layerwise pretraining
					workspaceMgr.assertNotOpen(ArrayType.UPDATER_WORKING_MEM, "Updater working memory")
				End If
				For Each ub As UpdaterBlock In updaterBlocks
					If ub.skipDueToPretrainConfig(TypeOf Me Is LayerUpdater) Then
						'Should skip some updater blocks sometimes
						'For example, VAE decoder params while doing supervised backprop
						Continue For
					End If
					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.UPDATER_WORKING_MEM)
						If isExternal Then
							'RL4J etc type case: calculate gradients in 1 net, update them in another
							ub.updateExternalGradient(iteration, epoch, gradient.gradient(), Params)
						Else
							'Standard case
							ub.update(iteration, epoch)
						End If
					End Using
				Next ub
			End SyncLock
		End Sub

		Protected Friend Overridable Sub divideByMinibatch(ByVal isExternal As Boolean, ByVal gradient As Gradient, ByVal batchSize As Integer)
			'Challenge here: most gradients are actual gradients, and should be divided by the minibatch to get the average
			'However, some 'gradients' are actually updates - an example being BatchNorm mean/variance estimates... these
			' shouldn't be modified

			If Not initializedMinibatchDivision Then
				gradientsForMinibatchDivision = getMinibatchDivisionSubsets(FlattenedGradientsView)
				initializedMinibatchDivision = True
			End If

			Dim toDivide As IList(Of INDArray)
			If isExternal Then
				toDivide = getMinibatchDivisionSubsets(gradient.gradient())
			Else
				toDivide = gradientsForMinibatchDivision
			End If
			For Each arr As INDArray In toDivide
				arr.divi(batchSize)
			Next arr
		End Sub

		Protected Friend Overridable Function getMinibatchDivisionSubsets(ByVal from As INDArray) As IList(Of INDArray)
			Dim [out] As IList(Of INDArray) = New List(Of INDArray)()
			Dim paramsSoFar As Long = 0
			Dim currentStart As Long = 0
			Dim currentEnd As Long = 0
			For Each t As Trainable In OrderedLayers
				Dim layerParams As ISet(Of String) = t.paramTable(False).Keys
				Dim paramTable As IDictionary(Of String, INDArray) = t.paramTable(False)
				For Each s As String In layerParams
					If t.updaterDivideByMinibatch(s) Then
						Dim l As Long = paramTable(s).length()
						currentEnd += l
					Else
						'This param/gradient subset should be excluded
						If currentEnd > currentStart Then
							Dim subset As INDArray = from.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(currentStart, currentEnd))
							[out].Add(subset)
						End If
						currentStart = paramsSoFar + paramTable(s).length()
						currentEnd = currentStart
					End If
					paramsSoFar += paramTable(s).length()
				Next s
			Next t

			If currentEnd > currentStart AndAlso currentStart < from.length() Then
				'Process last part of the gradient view array
				Dim subset As INDArray = from.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.interval(currentStart, currentEnd))
				[out].Add(subset)
			End If
			Return [out]
		End Function

		Protected Friend Overridable ReadOnly Property SingleLayerUpdater As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' Pre-apply: Apply gradient normalization/clipping
		''' </summary>
		''' <param name="layer">     Layer to apply gradient normalization/clipping for </param>
		''' <param name="gradient">  Gradient to update </param>
		''' <param name="iteration"> The current iteration (i.e., number of parameter updates so far) </param>
		Public Overridable Sub preApply(ByVal layer As Trainable, ByVal gradient As Gradient, ByVal iteration As Integer)

			If layer.Config Is Nothing OrElse layer.numParams() = 0 Then
				'Layer does not have parameters -> no gradient
				Return
			End If

			Dim normalization As GradientNormalization = layer.Config.GradientNormalization
			If normalization = Nothing OrElse normalization = GradientNormalization.None Then
				Return 'no op
			End If

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final double threshold = layer.getConfig().getGradientNormalizationThreshold();
			Dim threshold As Double = layer.Config.GradientNormalizationThreshold
			Dim layerGradientView As INDArray = layer.GradientsViewArray

			Select Case normalization
				Case GradientNormalization.RenormalizeL2PerLayer
					If layerGradientView IsNot Nothing Then
						Dim l2 As Double = layerGradientView.norm2Number().doubleValue()
						If l2 = 0.0 Then
							l2 = 1e-5 'Avoid 0/0 -> NaN
						End If
						layerGradientView.divi(l2)
					End If
				Case GradientNormalization.RenormalizeL2PerParamType
					For Each g As INDArray In gradient.gradientForVariable().Values
						Dim l2 As Double = Nd4j.Executioner.execAndReturn(New Norm2(g)).getFinalResult().doubleValue()
						If l2 = 0.0 Then
							l2 = 1e-5 'Avoid 0/0 -> NaN
						End If
						g.divi(l2)
					Next g
				Case GradientNormalization.ClipElementWiseAbsoluteValue
					If layerGradientView IsNot Nothing Then
						Dim op As CustomOp = DynamicCustomOp.builder("clipbyvalue").addInputs(layerGradientView).callInplace(True).addFloatingPointArguments(-threshold, threshold).build()
						Nd4j.Executioner.exec(op)
					End If
				Case GradientNormalization.ClipL2PerLayer
					If layerGradientView IsNot Nothing Then
						Dim layerL2 As Double = layerGradientView.norm2Number().doubleValue()
						If layerL2 > threshold Then
							Dim scalingFactor As Double = threshold / layerL2 ' g = g / l2 * threshold ->
							layerGradientView.muli(scalingFactor)
						End If
					End If
				Case GradientNormalization.ClipL2PerParamType
					For Each g As INDArray In gradient.gradientForVariable().Values
						Dim l2 As Double = g.norm2Number().doubleValue()
						If l2 > threshold Then
							Dim scalingFactor As Double = l2 / threshold
							g.divi(scalingFactor)
						End If
					Next g
				Case Else
					Throw New Exception("Unknown (or not implemented) gradient normalization strategy: " & normalization)
			End Select
		End Sub

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: BaseMultiLayerUpdater<?> that = (BaseMultiLayerUpdater<?>) o;
			Dim that As BaseMultiLayerUpdater(Of Object) = DirectCast(o, BaseMultiLayerUpdater(Of Object))
			Return If(updaterStateViewArray IsNot Nothing, updaterStateViewArray.Equals(that.updaterStateViewArray), that.updaterStateViewArray Is Nothing)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = If(layersByName IsNot Nothing, layersByName.GetHashCode(), 0)
			result = 31 * result + (If(updaterBlocks IsNot Nothing, updaterBlocks.GetHashCode(), 0))
			result = 31 * result + (If(updaterStateViewArray IsNot Nothing, updaterStateViewArray.GetHashCode(), 0))
			Return result
		End Function
	End Class

End Namespace