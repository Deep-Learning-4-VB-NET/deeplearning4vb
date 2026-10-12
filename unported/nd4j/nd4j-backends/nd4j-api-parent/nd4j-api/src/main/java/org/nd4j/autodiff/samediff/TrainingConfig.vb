Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports L1Regularization = org.nd4j.linalg.learning.regularization.L1Regularization
Imports L2Regularization = org.nd4j.linalg.learning.regularization.L2Regularization
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports WeightDecay = org.nd4j.linalg.learning.regularization.WeightDecay
Imports JsonMappers = org.nd4j.serde.json.JsonMappers

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

Namespace org.nd4j.autodiff.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Builder @NoArgsConstructor @AllArgsConstructor @Slf4j public class TrainingConfig
	Public Class TrainingConfig

		Private updater As IUpdater
		Private regularization As IList(Of Regularization) = New List(Of Regularization)() 'Regularization for all trainable parameters
		Private minimize As Boolean = True
		Private dataSetFeatureMapping As IList(Of String)
		Private dataSetLabelMapping As IList(Of String)
		Private dataSetFeatureMaskMapping As IList(Of String)
		Private dataSetLabelMaskMapping As IList(Of String)
		Private lossVariables As IList(Of String)
		Private iterationCount As Integer
		Private epochCount As Integer


		Private trainEvaluations As IDictionary(Of String, IList(Of IEvaluation)) = New Dictionary(Of String, IList(Of IEvaluation))()
		Private trainEvaluationLabels As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()

		Private validationEvaluations As IDictionary(Of String, IList(Of IEvaluation)) = New Dictionary(Of String, IList(Of IEvaluation))()
		Private validationEvaluationLabels As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()

		''' <summary>
		''' Create a training configuration suitable for training a single input, single output network.<br>
		''' See also the <seealso cref="Builder"/> for creating a TrainingConfig
		''' </summary>
		''' <param name="updater">               The updater configuration to use </param>
		''' <param name="dataSetFeatureMapping"> The name of the placeholder/variable that should be set using the feature INDArray from the DataSet
		'''                              (or the first/only feature from a MultiDataSet). For example, if the network input placeholder was
		'''                              called "input" then this should be set to "input" </param>
		''' <param name="dataSetLabelMapping">   The name of the placeholder/variable that should be set using the label INDArray from the DataSet
		'''                              (or the first/only feature from a MultiDataSet). For example, if the network input placeholder was
		'''                              called "input" then this should be set to "input" </param>
		Public Sub New(ByVal updater As IUpdater, ByVal regularization As IList(Of Regularization), ByVal dataSetFeatureMapping As String, ByVal dataSetLabelMapping As String)
			Me.New(updater, regularization, True, Collections.singletonList(dataSetFeatureMapping), Collections.singletonList(dataSetLabelMapping), Enumerable.Empty(Of String)(), Enumerable.Empty(Of String)(), Nothing)
		End Sub

		''' <summary>
		''' Create a training configuration suitable for training both single input/output and multi input/output networks.<br>
		''' See also the <seealso cref="Builder"/> for creating a TrainingConfig
		''' </summary>
		''' <param name="updater">                   The updater configuration to use </param>
		''' <param name="regularization">            Regularization for all trainable parameters;\ </param>
		''' <param name="minimize">                  Set to true if the loss function should be minimized (usually true). False to maximize </param>
		''' <param name="dataSetFeatureMapping">     The name of the placeholders/variables that should be set using the feature INDArray(s) from the
		'''                                  DataSet or MultiDataSet. For example, if the network had 2 inputs called "input1" and "input2"
		'''                                  and the MultiDataSet features should be mapped with {@code MultiDataSet.getFeatures(0)->"input1"}
		'''                                  and {@code MultiDataSet.getFeatures(1)->"input2"}, then this should be set to {@code List<>("input1", "input2")}. </param>
		''' <param name="dataSetLabelMapping">       As per dataSetFeatureMapping, but for the DataSet/MultiDataSet labels </param>
		''' <param name="dataSetFeatureMaskMapping"> May be null. If non-null, the variables that the MultiDataSet feature mask arrays should be associated with. </param>
		''' <param name="dataSetLabelMaskMapping">   May be null. If non-null, the variables that the MultiDataSet label mask arrays should be associated with. </param>
		Public Sub New(ByVal updater As IUpdater, ByVal regularization As IList(Of Regularization), ByVal minimize As Boolean, ByVal dataSetFeatureMapping As IList(Of String), ByVal dataSetLabelMapping As IList(Of String), ByVal dataSetFeatureMaskMapping As IList(Of String), ByVal dataSetLabelMaskMapping As IList(Of String), ByVal lossVariables As IList(Of String))
			Me.updater = updater
			Me.regularization = regularization
			Me.minimize = minimize
			Me.dataSetFeatureMapping = dataSetFeatureMapping
			Me.dataSetLabelMapping = dataSetLabelMapping
			Me.dataSetFeatureMaskMapping = dataSetFeatureMaskMapping
			Me.dataSetLabelMaskMapping = dataSetLabelMaskMapping
			Me.lossVariables = lossVariables
		End Sub

		Protected Friend Sub New(ByVal updater As IUpdater, ByVal regularization As IList(Of Regularization), ByVal minimize As Boolean, ByVal dataSetFeatureMapping As IList(Of String), ByVal dataSetLabelMapping As IList(Of String), ByVal dataSetFeatureMaskMapping As IList(Of String), ByVal dataSetLabelMaskMapping As IList(Of String), ByVal lossVariables As IList(Of String), ByVal trainEvaluations As IDictionary(Of String, IList(Of IEvaluation)), ByVal trainEvaluationLabels As IDictionary(Of String, Integer), ByVal validationEvaluations As IDictionary(Of String, IList(Of IEvaluation)), ByVal validationEvaluationLabels As IDictionary(Of String, Integer))
			Me.New(updater, regularization, minimize, dataSetFeatureMapping, dataSetLabelMapping, dataSetFeatureMaskMapping, dataSetLabelMaskMapping, lossVariables)
			Me.trainEvaluations = trainEvaluations
			Me.trainEvaluationLabels = trainEvaluationLabels
			Me.validationEvaluations = validationEvaluations
			Me.validationEvaluationLabels = validationEvaluationLabels
		End Sub

		''' <summary>
		''' Increment the iteration count by 1
		''' </summary>
		Public Overridable Sub incrementIterationCount()
			iterationCount += 1
		End Sub

		''' <summary>
		''' Increment the epoch count by 1
		''' </summary>
		Public Overridable Sub incrementEpochCount()
			epochCount += 1
		End Sub

		Public Shared Function builder() As Builder
			Return New Builder()
		End Function

		''' <summary>
		''' Get the index of the label array that the specified variable is associated with </summary>
		''' <param name="s"> Name of the variable </param>
		''' <returns> The index of the label variable, or -1 if not found </returns>
		Public Overridable Function labelIdx(ByVal s As String) As Integer
			Return dataSetLabelMapping.IndexOf(s)
		End Function

		Public Class Builder

'JAVA TO VB CONVERTER NOTE: The field updater was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend updater_Conflict As IUpdater
'JAVA TO VB CONVERTER NOTE: The field regularization was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend regularization_Conflict As IList(Of Regularization) = New List(Of Regularization)()
'JAVA TO VB CONVERTER NOTE: The field minimize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend minimize_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field dataSetFeatureMapping was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend dataSetFeatureMapping_Conflict As IList(Of String)
'JAVA TO VB CONVERTER NOTE: The field dataSetLabelMapping was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend dataSetLabelMapping_Conflict As IList(Of String)
'JAVA TO VB CONVERTER NOTE: The field dataSetFeatureMaskMapping was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend dataSetFeatureMaskMapping_Conflict As IList(Of String)
'JAVA TO VB CONVERTER NOTE: The field dataSetLabelMaskMapping was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend dataSetLabelMaskMapping_Conflict As IList(Of String)
			Friend lossVariables As IList(Of String)
			Friend skipValidation As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field markLabelsUnused was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend markLabelsUnused_Conflict As Boolean = False

			Friend trainEvaluations As IDictionary(Of String, IList(Of IEvaluation)) = New Dictionary(Of String, IList(Of IEvaluation))()
			Friend trainEvaluationLabels As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()

			Friend validationEvaluations As IDictionary(Of String, IList(Of IEvaluation)) = New Dictionary(Of String, IList(Of IEvaluation))()
			Friend validationEvaluationLabels As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()

			''' <summary>
			''' Set the updater (such as <seealso cref="org.nd4j.linalg.learning.config.Adam"/>, <seealso cref="org.nd4j.linalg.learning.config.Nesterovs"/>
			''' etc. This is also how the learning rate (or learning rate schedule) is set. </summary>
			''' <param name="updater">  Updater to set </param>
'JAVA TO VB CONVERTER NOTE: The parameter updater was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function updater(ByVal updater_Conflict As IUpdater) As Builder
				Me.updater_Conflict = updater_Conflict
				Return Me
			End Function

			''' <summary>
			''' Sets the L1 regularization coefficient for all trainable parameters. Must be >= 0.<br>
			''' See <seealso cref="L1Regularization"/> for more details </summary>
			''' <param name="l1"> L1 regularization coefficient </param>
'JAVA TO VB CONVERTER NOTE: The parameter l1 was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function l1(ByVal l1_Conflict As Double) As Builder
				Preconditions.checkState(l1_Conflict >= 0, "L1 regularization coefficient must be >= 0. Got %s", l1_Conflict)
				removeInstances(Me.regularization_Conflict, GetType(L1Regularization))
				Me.regularization_Conflict.Add(New L1Regularization(l1_Conflict))
				Return Me
			End Function

			''' <summary>
			''' Sets the L2 regularization coefficient for all trainable parameters. Must be >= 0.<br>
			''' <b>Note</b>: Generally, <seealso cref="WeightDecay"/> (set via <seealso cref="weightDecay(Double,Boolean)"/> should be preferred to
			''' L2 regularization. See <seealso cref="WeightDecay"/> javadoc for further details.<br>
			''' Note: L2 regularization and weight decay usually should not be used together; if any weight decay (or L2) has
			''' been added for the biases, these will be removed first.
			''' </summary>
			''' <seealso cref= #weightDecay(double, boolean) </seealso>
'JAVA TO VB CONVERTER NOTE: The parameter l2 was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function l2(ByVal l2_Conflict As Double) As Builder
				Preconditions.checkState(l2_Conflict >= 0.0, "L2 regularization coefficient must be >= 0. Got %s", l2_Conflict)
				'Check if existing L2 exists; if so, replace it. Also remove weight decay - it doesn't make sense to use both
				removeInstances(Me.regularization_Conflict, GetType(L2Regularization))
				If l2_Conflict > 0.0 Then
					removeInstancesWithWarning(Me.regularization_Conflict, GetType(WeightDecay), "WeightDecay regularization removed: incompatible with added L2 regularization")
					Me.regularization_Conflict.Add(New L2Regularization(l2_Conflict))
				End If
				Return Me
			End Function

			''' <summary>
			''' Add weight decay regularization for all trainable parameters. See <seealso cref="WeightDecay"/> for more details.<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.<br>
			''' </summary>
			''' <param name="coefficient"> Weight decay regularization coefficient </param>
			''' <param name="applyLR">     Whether the learning rate should be multiplied in when performing weight decay updates. See <seealso cref="WeightDecay"/> for more details. </param>
			Public Overridable Function weightDecay(ByVal coefficient As Double, ByVal applyLR As Boolean) As Builder
				'Check if existing weight decay if it exists; if so, replace it. Also remove L2 - it doesn't make sense to use both
				removeInstances(Me.regularization_Conflict, GetType(WeightDecay))
				If coefficient > 0.0 Then
					removeInstancesWithWarning(Me.regularization_Conflict, GetType(L2Regularization), "L2 regularization removed: incompatible with added WeightDecay regularization")
					Me.regularization_Conflict.Add(New WeightDecay(coefficient, applyLR))
				End If
				Return Me
			End Function

			''' <summary>
			''' Add regularization to all trainable parameters in the network
			''' </summary>
			''' <param name="regularizations"> Regularization type(s) to add </param>
			Public Overridable Function addRegularization(ParamArray ByVal regularizations() As Regularization) As Builder
				Collections.addAll(Me.regularization_Conflict, regularizations)
				Return Me
			End Function

			''' <summary>
			''' Set the regularization for all trainable parameters in the network.
			''' Note that if any existing regularization types have been added, they will be removed
			''' </summary>
			''' <param name="regularization"> Regularization type(s) to add </param>
'JAVA TO VB CONVERTER NOTE: The parameter regularization was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function regularization(ParamArray ByVal regularization_Conflict() As Regularization) As Builder
				If regularization_Conflict Is Nothing OrElse regularization_Conflict.Length = 0 Then
					Return Me
				End If
				Dim r As IList(Of Regularization) = New List(Of Regularization)()
				Collections.addAll(r, regularization_Conflict)
				Return Me.regularization(r)
			End Function

			''' <summary>
			''' Set the regularization for all trainable parameters in the network.
			''' Note that if any existing regularization types have been added, they will be removed
			''' </summary>
			''' <param name="regularization"> Regularization type(s) to add </param>
'JAVA TO VB CONVERTER NOTE: The parameter regularization was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function regularization(ByVal regularization_Conflict As IList(Of Regularization)) As Builder
				Me.regularization_Conflict = regularization_Conflict
				Return Me
			End Function

			''' <summary>
			''' Sets whether the loss function should be minimized (true) or maximized (false).<br>
			''' The loss function is usually minimized in SGD.<br>
			''' Default: true. </summary>
			''' <param name="minimize"> True to minimize, false to maximize </param>
'JAVA TO VB CONVERTER NOTE: The parameter minimize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function minimize(ByVal minimize_Conflict As Boolean) As Builder
				Me.minimize_Conflict = minimize_Conflict
				Return Me
			End Function

			''' <summary>
			''' Set the name of the placeholders/variables that should be set using the feature INDArray(s) from the
			''' DataSet or MultiDataSet. For example, if the network had 2 inputs called "input1" and "input2"
			''' and the MultiDataSet features should be mapped with {@code MultiDataSet.getFeatures(0)->"input1"}
			''' and {@code MultiDataSet.getFeatures(1)->"input2"}, then this should be set to {@code List<>("input1", "input2")}.
			''' </summary>
			''' <param name="dataSetFeatureMapping"> Name of the variables/placeholders that the feature arrays should be mapped to </param>
'JAVA TO VB CONVERTER NOTE: The parameter dataSetFeatureMapping was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dataSetFeatureMapping(ParamArray ByVal dataSetFeatureMapping_Conflict() As String) As Builder
				Return Me.dataSetFeatureMapping(java.util.Arrays.asList(dataSetFeatureMapping_Conflict))
			End Function

			''' <summary>
			''' Set the name of the placeholders/variables that should be set using the feature INDArray(s) from the
			''' DataSet or MultiDataSet. For example, if the network had 2 inputs called "input1" and "input2"
			''' and the MultiDataSet features should be mapped with {@code MultiDataSet.getFeatures(0)->"input1"}
			''' and {@code MultiDataSet.getFeatures(1)->"input2"}, then this should be set to {@code "input1", "input2"}.
			''' </summary>
			''' <param name="dataSetFeatureMapping"> Name of the variables/placeholders that the feature arrays should be mapped to </param>
'JAVA TO VB CONVERTER NOTE: The parameter dataSetFeatureMapping was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dataSetFeatureMapping(ByVal dataSetFeatureMapping_Conflict As IList(Of String)) As Builder
				Preconditions.checkNotNull(dataSetFeatureMapping_Conflict IsNot Nothing AndAlso dataSetFeatureMapping_Conflict.Count > 0, "No feature mapping was provided")
				Me.dataSetFeatureMapping_Conflict = dataSetFeatureMapping_Conflict
				Return Me
			End Function

			''' <summary>
			''' Set the name of the placeholders/variables that should be set using the labels INDArray(s) from the
			''' DataSet or MultiDataSet. For example, if the network had 2 labels called "label1" and "label2"
			''' and the MultiDataSet labels should be mapped with {@code MultiDataSet.getLabel(0)->"label1"}
			''' and {@code MultiDataSet.getLabels(1)->"label"}, then this should be set to {@code "label1", "label2"}.
			''' </summary>
			''' <param name="dataSetLabelMapping"> Name of the variables/placeholders that the label arrays should be mapped to </param>
'JAVA TO VB CONVERTER NOTE: The parameter dataSetLabelMapping was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dataSetLabelMapping(ParamArray ByVal dataSetLabelMapping_Conflict() As String) As Builder
				Return Me.dataSetLabelMapping(java.util.Arrays.asList(dataSetLabelMapping_Conflict))
			End Function

			''' <summary>
			''' Set the name of the placeholders/variables that should be set using the labels INDArray(s) from the
			''' DataSet or MultiDataSet. For example, if the network had 2 labels called "label1" and "label2"
			''' and the MultiDataSet labels should be mapped with {@code MultiDataSet.getLabel(0)->"label1"}
			''' and {@code MultiDataSet.getLabels(1)->"label"}, then this should be set to {@code "label1", "label2"}.
			''' </summary>
			''' <param name="dataSetLabelMapping"> Name of the variables/placeholders that the label arrays should be mapped to </param>
'JAVA TO VB CONVERTER NOTE: The parameter dataSetLabelMapping was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dataSetLabelMapping(ByVal dataSetLabelMapping_Conflict As IList(Of String)) As Builder
				Preconditions.checkNotNull(dataSetLabelMapping_Conflict IsNot Nothing AndAlso dataSetLabelMapping_Conflict.Count > 0, "No label mapping was provided")
				Me.dataSetLabelMapping_Conflict = dataSetLabelMapping_Conflict
				Return Me
			End Function

			''' <summary>
			''' Calling this method will mark the label as unused. This is basically a way to turn off label mapping validation in
			''' TrainingConfig builder, for training models without labels.<br>
			''' Put another way: usually you need to call <seealso cref="dataSetLabelMapping(String...)"/> to set labels, this method
			''' allows you to say that the DataSet/MultiDataSet labels aren't used in training.
			''' </summary>
			Public Overridable Function markLabelsUnused() As Builder
				Me.markLabelsUnused_Conflict = True
				Return Me
			End Function

			''' <summary>
			''' See <seealso cref="dataSetFeatureMaskMapping(List)"/>
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter dataSetFeatureMaskMapping was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dataSetFeatureMaskMapping(ParamArray ByVal dataSetFeatureMaskMapping_Conflict() As String) As Builder
				Return Me.dataSetFeatureMaskMapping(java.util.Arrays.asList(dataSetFeatureMaskMapping_Conflict))
			End Function

			''' <summary>
			''' Set the name of the placeholders/variables that should be set using the feature mask INDArray(s) from the
			''' DataSet or MultiDataSet. For example, if the network had 2 mask variables called "mask1" and "mask2"
			''' and the MultiDataSet features masks should be mapped with {@code MultiDataSet.getFeatureMaskArray(0)->"mask1"}
			''' and {@code MultiDataSet.getFeatureMaskArray(1)->"mask2"}, then this should be set to {@code "mask1", "mask2"}.
			''' </summary>
			''' <param name="dataSetFeatureMaskMapping"> Name of the variables/placeholders that the feature arrays should be mapped to </param>
'JAVA TO VB CONVERTER NOTE: The parameter dataSetFeatureMaskMapping was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dataSetFeatureMaskMapping(ByVal dataSetFeatureMaskMapping_Conflict As IList(Of String)) As Builder
				Me.dataSetFeatureMaskMapping_Conflict = dataSetFeatureMaskMapping_Conflict
				Return Me
			End Function

			''' <summary>
			''' See <seealso cref="dataSetLabelMaskMapping(List)"/>
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter dataSetLabelMaskMapping was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dataSetLabelMaskMapping(ParamArray ByVal dataSetLabelMaskMapping_Conflict() As String) As Builder
				Return Me.dataSetLabelMaskMapping(java.util.Arrays.asList(dataSetLabelMaskMapping_Conflict))
			End Function

			''' <summary>
			''' Set the name of the placeholders/variables that should be set using the label mask INDArray(s) from the
			''' DataSet or MultiDataSet. For example, if the network had 2 mask variables called "mask1" and "mask2"
			''' and the MultiDataSet label masks should be mapped with {@code MultiDataSet.getLabelMaskArray(0)->"mask1"}
			''' and {@code MultiDataSet.getLabelMaskArray(1)->"mask2"}, then this should be set to {@code "mask1", "mask2"}.
			''' </summary>
			''' <param name="dataSetLabelMaskMapping"> Name of the variables/placeholders that the feature arrays should be mapped to </param>
'JAVA TO VB CONVERTER NOTE: The parameter dataSetLabelMaskMapping was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dataSetLabelMaskMapping(ByVal dataSetLabelMaskMapping_Conflict As IList(Of String)) As Builder
				Me.dataSetLabelMaskMapping_Conflict = dataSetLabelMaskMapping_Conflict
				Return Me
			End Function

			Public Overridable Function skipBuilderValidation(ByVal skip As Boolean) As Builder
				Me.skipValidation = skip
				Return Me
			End Function

			Public Overridable Function minimize(ParamArray ByVal lossVariables() As String) As Builder
				Me.lossVariables = New List(Of String) From {lossVariables}
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private void addEvaluations(boolean validation, @NonNull Map<String, List<org.nd4j.evaluation.IEvaluation>> evaluationMap, @NonNull Map<String, Integer> labelMap, @NonNull String variableName, int labelIndex, @NonNull IEvaluation... evaluations)
			Friend Overridable Sub addEvaluations(ByVal validation As Boolean, ByVal evaluationMap As IDictionary(Of String, IList(Of IEvaluation)), ByVal labelMap As IDictionary(Of String, Integer), ByVal variableName As String, ByVal labelIndex As Integer, ParamArray ByVal evaluations() As IEvaluation)
				If evaluationMap.ContainsKey(variableName) AndAlso labelMap(variableName) <> labelIndex Then
					Dim s As String

					If validation Then
						s = "This ListenerEvaluations.Builder already has validation evaluations for "
					Else
						s = "This ListenerEvaluations.Builder already has train evaluations for "
					End If

					Throw New System.ArgumentException(s & "variable " & variableName & " with label index " & labelIndex & ".  You can't add " & " evaluations with a different label index.  Got label index " & labelIndex)
				End If

				If evaluationMap.ContainsKey(variableName) Then
					CType(evaluationMap(variableName), List(Of IEvaluation)).AddRange(New List(Of IEvaluation) From {evaluations})
				Else
					evaluationMap(variableName) = New List(Of IEvaluation) From {evaluations}
					labelMap(variableName) = labelIndex
				End If
			End Sub

			''' <summary>
			''' Add requested History training evaluations for a parm/variable.
			''' 
			''' These evaluations will be reported in the <seealso cref="org.nd4j.autodiff.listeners.records.History"/> object returned by fit.
			''' </summary>
			''' <param name="variableName">  The variable to evaluate </param>
			''' <param name="labelIndex">    The index of the label to evaluate against </param>
			''' <param name="evaluations">   The evaluations to run </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder trainEvaluation(@NonNull String variableName, int labelIndex, @NonNull IEvaluation... evaluations)
			Public Overridable Function trainEvaluation(ByVal variableName As String, ByVal labelIndex As Integer, ParamArray ByVal evaluations() As IEvaluation) As Builder
				addEvaluations(False, Me.trainEvaluations, Me.trainEvaluationLabels, variableName, labelIndex, evaluations)
				Return Me
			End Function

			''' <summary>
			''' Add requested History training evaluations for a parm/variable.
			''' 
			''' These evaluations will be reported in the <seealso cref="org.nd4j.autodiff.listeners.records.History"/> object returned by fit.
			''' </summary>
			''' <param name="variable">      The variable to evaluate </param>
			''' <param name="labelIndex">    The index of the label to evaluate against </param>
			''' <param name="evaluations">   The evaluations to run </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder trainEvaluation(@NonNull SDVariable variable, int labelIndex, @NonNull IEvaluation... evaluations)
			Public Overridable Function trainEvaluation(ByVal variable As SDVariable, ByVal labelIndex As Integer, ParamArray ByVal evaluations() As IEvaluation) As Builder
				Return trainEvaluation(variable.name(), labelIndex, evaluations)
			End Function

			''' <summary>
			''' Add requested History validation evaluations for a parm/variable.
			''' 
			''' These evaluations will be reported in the <seealso cref="org.nd4j.autodiff.listeners.records.History"/> object returned by fit.
			''' </summary>
			''' <param name="variableName">  The variable to evaluate </param>
			''' <param name="labelIndex">    The index of the label to evaluate against </param>
			''' <param name="evaluations">   The evaluations to run </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder validationEvaluation(@NonNull String variableName, int labelIndex, @NonNull IEvaluation... evaluations)
			Public Overridable Function validationEvaluation(ByVal variableName As String, ByVal labelIndex As Integer, ParamArray ByVal evaluations() As IEvaluation) As Builder
				addEvaluations(True, Me.validationEvaluations, Me.validationEvaluationLabels, variableName, labelIndex, evaluations)
				Return Me
			End Function

			''' <summary>
			''' Add requested History validation evaluations for a parm/variable.
			''' 
			''' These evaluations will be reported in the <seealso cref="org.nd4j.autodiff.listeners.records.History"/> object returned by fit.
			''' </summary>
			''' <param name="variable">      The variable to evaluate </param>
			''' <param name="labelIndex">    The index of the label to evaluate against </param>
			''' <param name="evaluations">   The evaluations to run </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder validationEvaluation(@NonNull SDVariable variable, int labelIndex, @NonNull IEvaluation... evaluations)
			Public Overridable Function validationEvaluation(ByVal variable As SDVariable, ByVal labelIndex As Integer, ParamArray ByVal evaluations() As IEvaluation) As Builder
				Return validationEvaluation(variable.name(), labelIndex, evaluations)
			End Function

			''' <summary>
			''' Add requested evaluations for a parm/variable, for either training or validation.
			''' 
			''' These evaluations will be reported in the <seealso cref="org.nd4j.autodiff.listeners.records.History"/> object returned by fit.
			''' </summary>
			''' <param name="validation">    Whether to add these evaluations as validation or training </param>
			''' <param name="variableName">  The variable to evaluate </param>
			''' <param name="labelIndex">    The index of the label to evaluate against </param>
			''' <param name="evaluations">   The evaluations to run </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder addEvaluations(boolean validation, @NonNull String variableName, int labelIndex, @NonNull IEvaluation... evaluations)
			Public Overridable Function addEvaluations(ByVal validation As Boolean, ByVal variableName As String, ByVal labelIndex As Integer, ParamArray ByVal evaluations() As IEvaluation) As Builder
				If validation Then
					Return validationEvaluation(variableName, labelIndex, evaluations)
				Else
					Return trainEvaluation(variableName, labelIndex, evaluations)
				End If
			End Function

			Public Overridable Function build() As TrainingConfig
				If Not skipValidation Then
					Preconditions.checkState(updater_Conflict IsNot Nothing, "Updater (optimizer) must not be null. Use updater(IUpdater) to set an updater")
					Preconditions.checkState(dataSetFeatureMapping_Conflict IsNot Nothing, "No DataSet feature mapping has been provided. A " & "mapping between DataSet array positions and variables/placeholders must be provided - use dateSetFeatureMapping(...) to set this")
					Preconditions.checkState(markLabelsUnused_Conflict OrElse dataSetLabelMapping_Conflict IsNot Nothing, "No DataSet label mapping has been provided. A " & "mapping between DataSet array positions and variables/placeholders must be provided - use dataSetLabelMapping(...) to set this," & " or use markLabelsUnused() to mark labels as unused (for example, for unsupervised learning)")


					Preconditions.checkArgument(trainEvaluations.Keys.Equals(trainEvaluationLabels.Keys), "Must specify a label index for each train evaluation.  Expected: %s, got: %s", trainEvaluations.Keys, trainEvaluationLabels.Keys)

					Preconditions.checkArgument(validationEvaluations.Keys.Equals(validationEvaluationLabels.Keys), "Must specify a label index for each validation evaluation.  Expected: %s, got: %s", validationEvaluations.Keys, validationEvaluationLabels.Keys)
				End If

				Return New TrainingConfig(updater_Conflict, regularization_Conflict, minimize_Conflict, dataSetFeatureMapping_Conflict, dataSetLabelMapping_Conflict, dataSetFeatureMaskMapping_Conflict, dataSetLabelMaskMapping_Conflict, lossVariables, trainEvaluations, trainEvaluationLabels, validationEvaluations, validationEvaluationLabels)
			End Function
		End Class


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


		Public Overridable Function toJson() As String
			Try
				Return JsonMappers.Mapper.writeValueAsString(Me)
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static TrainingConfig fromJson(@NonNull String json)
		Public Shared Function fromJson(ByVal json As String) As TrainingConfig
			Try
				Return JsonMappers.Mapper.readValue(json, GetType(TrainingConfig))
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function
	End Class

End Namespace