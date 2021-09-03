Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Updater = org.deeplearning4j.nn.conf.Updater
Imports Distribution = org.deeplearning4j.nn.conf.distribution.Distribution
Imports BaseLayer = org.deeplearning4j.nn.conf.layers.BaseLayer
Imports BaseOutputLayer = org.deeplearning4j.nn.conf.layers.BaseOutputLayer
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports org.deeplearning4j.nn.weights
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports org.nd4j.linalg.activations.impl
Imports org.nd4j.linalg.learning.config
Imports L1Regularization = org.nd4j.linalg.learning.regularization.L1Regularization
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports WeightDecay = org.nd4j.linalg.learning.regularization.WeightDecay
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports org.nd4j.linalg.lossfunctions.impl
Imports JsonParser = org.nd4j.shade.jackson.core.JsonParser
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException
Imports DeserializationContext = org.nd4j.shade.jackson.databind.DeserializationContext
Imports JsonDeserializer = org.nd4j.shade.jackson.databind.JsonDeserializer
Imports JsonMappingException = org.nd4j.shade.jackson.databind.JsonMappingException
Imports ResolvableDeserializer = org.nd4j.shade.jackson.databind.deser.ResolvableDeserializer
Imports StdDeserializer = org.nd4j.shade.jackson.databind.deser.std.StdDeserializer
Imports ObjectNode = org.nd4j.shade.jackson.databind.node.ObjectNode

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

Namespace org.deeplearning4j.nn.conf.serde


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseNetConfigDeserializer<T> extends org.nd4j.shade.jackson.databind.deser.std.StdDeserializer<T> implements org.nd4j.shade.jackson.databind.deser.ResolvableDeserializer
	Public MustInherit Class BaseNetConfigDeserializer(Of T)
		Inherits StdDeserializer(Of T)
		Implements ResolvableDeserializer

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: protected final org.nd4j.shade.jackson.databind.JsonDeserializer<?> defaultDeserializer;
		Protected Friend ReadOnly defaultDeserializer As JsonDeserializer(Of Object)

'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
'ORIGINAL LINE: public BaseNetConfigDeserializer(org.nd4j.shade.jackson.databind.JsonDeserializer<?> defaultDeserializer, @Class<T> deserializedType)
		Public Sub New(ByVal defaultDeserializer As JsonDeserializer(Of T1), ByVal deserializedType As Type(Of T))
			MyBase.New(deserializedType)
			Me.defaultDeserializer = defaultDeserializer
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public abstract T deserialize(org.nd4j.shade.jackson.core.JsonParser jp, org.nd4j.shade.jackson.databind.DeserializationContext ctxt) throws IOException, org.nd4j.shade.jackson.core.JsonProcessingException;
		Public MustOverride Overrides Function deserialize(ByVal jp As JsonParser, ByVal ctxt As DeserializationContext) As T

		Protected Friend Overridable Function requiresIUpdaterFromLegacy(ByVal layers() As Layer) As Boolean
			For Each l As Layer In layers
				If TypeOf l Is BaseLayer Then
					Dim bl As BaseLayer = DirectCast(l, BaseLayer)
					If bl.getIUpdater() Is Nothing AndAlso bl.initializer().numParams(bl) > 0 Then
						Return True
					End If
				End If
			Next l
			Return False
		End Function

		Protected Friend Overridable Function requiresDropoutFromLegacy(ByVal layers() As Layer) As Boolean
			For Each l As Layer In layers
				If l.getIDropout() IsNot Nothing Then
					Return False
				End If
			Next l
			Return True
		End Function

		Protected Friend Overridable Function requiresRegularizationFromLegacy(ByVal layers() As Layer) As Boolean
			For Each l As Layer In layers
				If TypeOf l Is BaseLayer AndAlso DirectCast(l, BaseLayer).getRegularization() Is Nothing Then
					Return True
				End If
			Next l
			Return False
		End Function

		Protected Friend Overridable Function requiresWeightInitFromLegacy(ByVal layers() As Layer) As Boolean
			For Each l As Layer In layers
				If TypeOf l Is BaseLayer AndAlso DirectCast(l, BaseLayer).getWeightInitFn() Is Nothing Then
					Return True
				End If
			Next l
			Return False
		End Function

		Protected Friend Overridable Function requiresActivationFromLegacy(ByVal layers() As Layer) As Boolean
			For Each l As Layer In layers
				If TypeOf l Is BaseLayer AndAlso DirectCast(l, BaseLayer).getActivationFn() Is Nothing Then
					Return True
				End If
			Next l
			Return False
		End Function

		Protected Friend Overridable Function requiresLegacyLossHandling(ByVal layers() As Layer) As Boolean
			For Each l As Layer In layers
				If TypeOf l Is BaseOutputLayer AndAlso DirectCast(l, BaseOutputLayer).getLossFn() Is Nothing Then
					Return True
				End If
			Next l
			Return False
		End Function

		Protected Friend Overridable Sub handleUpdaterBackwardCompatibility(ByVal layer As BaseLayer, ByVal [on] As ObjectNode)
			If [on] IsNot Nothing AndAlso [on].has("updater") Then
				Dim updaterName As String = [on].get("updater").asText()
				If updaterName IsNot Nothing Then
					Dim u As Updater = Updater.valueOf(updaterName)
					Dim iu As IUpdater = u.getIUpdaterWithDefaultConfig()
					Dim lr As Double = [on].get("learningRate").asDouble()
					Dim eps As Double
					If [on].has("epsilon") Then
						eps = [on].get("epsilon").asDouble()
					Else
						eps = Double.NaN
					End If
					Dim rho As Double = [on].get("rho").asDouble()
					Select Case u.innerEnumValue
						Case Updater.InnerEnum.SGD
							DirectCast(iu, Sgd).setLearningRate(lr)
						Case Updater.InnerEnum.ADAM
							If Double.IsNaN(eps) Then
								eps = Adam.DEFAULT_ADAM_EPSILON
							End If
							DirectCast(iu, Adam).setLearningRate(lr)
							DirectCast(iu, Adam).setBeta1([on].get("adamMeanDecay").asDouble())
							DirectCast(iu, Adam).setBeta2([on].get("adamVarDecay").asDouble())
							DirectCast(iu, Adam).setEpsilon(eps)
						Case Updater.InnerEnum.ADAMAX
							If Double.IsNaN(eps) Then
								eps = AdaMax.DEFAULT_ADAMAX_EPSILON
							End If
							DirectCast(iu, AdaMax).setLearningRate(lr)
							DirectCast(iu, AdaMax).setBeta1([on].get("adamMeanDecay").asDouble())
							DirectCast(iu, AdaMax).setBeta2([on].get("adamVarDecay").asDouble())
							DirectCast(iu, AdaMax).setEpsilon(eps)
						Case Updater.InnerEnum.ADADELTA
							If Double.IsNaN(eps) Then
								eps = AdaDelta.DEFAULT_ADADELTA_EPSILON
							End If
							DirectCast(iu, AdaDelta).setRho(rho)
							DirectCast(iu, AdaDelta).setEpsilon(eps)
						Case Updater.InnerEnum.NESTEROVS
							DirectCast(iu, Nesterovs).setLearningRate(lr)
							DirectCast(iu, Nesterovs).setMomentum([on].get("momentum").asDouble())
						Case Updater.InnerEnum.NADAM
							If Double.IsNaN(eps) Then
								eps = Nadam.DEFAULT_NADAM_EPSILON
							End If
							DirectCast(iu, Nadam).setLearningRate(lr)
							DirectCast(iu, Nadam).setBeta1([on].get("adamMeanDecay").asDouble())
							DirectCast(iu, Nadam).setBeta2([on].get("adamVarDecay").asDouble())
							DirectCast(iu, Nadam).setEpsilon(eps)
						Case Updater.InnerEnum.ADAGRAD
							If Double.IsNaN(eps) Then
								eps = AdaGrad.DEFAULT_ADAGRAD_EPSILON
							End If
							DirectCast(iu, AdaGrad).setLearningRate(lr)
							DirectCast(iu, AdaGrad).setEpsilon(eps)
						Case Updater.InnerEnum.RMSPROP
							If Double.IsNaN(eps) Then
								eps = RmsProp.DEFAULT_RMSPROP_EPSILON
							End If
							DirectCast(iu, RmsProp).setLearningRate(lr)
							DirectCast(iu, RmsProp).setEpsilon(eps)
							DirectCast(iu, RmsProp).setRmsDecay([on].get("rmsDecay").asDouble())
						Case Else
							'No op
					End Select

					layer.setIUpdater(iu)
				End If
			End If
		End Sub

		Protected Friend Overridable Sub handleL1L2BackwardCompatibility(ByVal baseLayer As BaseLayer, ByVal [on] As ObjectNode)
			If [on] IsNot Nothing AndAlso ([on].has("l1") OrElse [on].has("l2")) Then
				'Legacy format JSON
				baseLayer.setRegularization(New List(Of Regularization)())
				baseLayer.setRegularizationBias(New List(Of Regularization)())

				If [on].has("l1") Then
					Dim l1 As Double = [on].get("l1").doubleValue()
					If l1 > 0.0 Then
						baseLayer.getRegularization().add(New L1Regularization(l1))
					End If
				End If
				If [on].has("l2") Then
					Dim l2 As Double = [on].get("l2").doubleValue()
					If l2 > 0.0 Then
						'Default to non-LR based WeightDecay, to match behaviour in 1.0.0-beta3
						baseLayer.getRegularization().add(New WeightDecay(l2, False))
					End If
				End If
				If [on].has("l1Bias") Then
					Dim l1Bias As Double = [on].get("l1Bias").doubleValue()
					If l1Bias > 0.0 Then
						baseLayer.getRegularizationBias().add(New L1Regularization(l1Bias))
					End If
				End If
				If [on].has("l2Bias") Then
					Dim l2Bias As Double = [on].get("l2Bias").doubleValue()
					If l2Bias > 0.0 Then
						'Default to non-LR based WeightDecay, to match behaviour in 1.0.0-beta3
						baseLayer.getRegularizationBias().add(New WeightDecay(l2Bias, False))
					End If
				End If
			End If
		End Sub

		Protected Friend Overridable Sub handleWeightInitBackwardCompatibility(ByVal baseLayer As BaseLayer, ByVal [on] As ObjectNode)
			If [on] IsNot Nothing AndAlso [on].has("weightInit") Then
				'Legacy format JSON
				If [on].has("weightInit") Then
					Dim wi As String = [on].get("weightInit").asText()
					Try
						Dim w As WeightInit = WeightInit.valueOf(wi)
						Dim d As Distribution = Nothing
						If w = WeightInit.DISTRIBUTION AndAlso [on].has("dist") Then
							Dim dist As String = [on].get("dist").ToString()
							d = NeuralNetConfiguration.mapper().readValue(dist, GetType(Distribution))
						End If
						Dim iwi As IWeightInit = w.getWeightInitFunction(d)
						baseLayer.setWeightInitFn(iwi)
					Catch t As Exception
						log.warn("Failed to infer weight initialization from legacy JSON format",t)
					End Try
				End If
			End If
		End Sub

		'Changed after 0.7.1 from "activationFunction" : "softmax" to "activationFn" : <object>
		Protected Friend Overridable Sub handleActivationBackwardCompatibility(ByVal baseLayer As BaseLayer, ByVal [on] As ObjectNode)
			If baseLayer.getActivationFn() Is Nothing AndAlso [on].has("activationFunction") Then
				Dim afn As String = [on].get("activationFunction").asText()
				Dim a As IActivation = Nothing
				Try
					a = getMap()(afn.ToLower()).getDeclaredConstructor().newInstance()
				Catch instantiationException As Exception When TypeOf instantiationException Is InstantiationException OrElse TypeOf instantiationException Is IllegalAccessException OrElse TypeOf instantiationException Is NoSuchMethodException OrElse TypeOf instantiationException Is InvocationTargetException
					log.error(instantiationException.getMessage())
				End Try
				baseLayer.setActivationFn(a)
			End If
		End Sub

		'0.5.0 and earlier: loss function was an enum like "lossFunction" : "NEGATIVELOGLIKELIHOOD",
		Protected Friend Overridable Sub handleLossBackwardCompatibility(ByVal baseLayer As BaseOutputLayer, ByVal [on] As ObjectNode)
			If baseLayer.getLossFn() Is Nothing AndAlso [on].has("activationFunction") Then
				Dim lfn As String = [on].get("lossFunction").asText()
				Dim loss As ILossFunction = Nothing
				Select Case lfn
					Case "MCXENT"
						loss = New LossMCXENT()
					Case "MSE"
						loss = New LossMSE()
					Case "NEGATIVELOGLIKELIHOOD"
						loss = New LossNegativeLogLikelihood()
					Case "SQUARED_LOSS"
						loss = New LossL2()
					Case "XENT"
						loss = New LossBinaryXENT()
				End Select
				baseLayer.setLossFn(loss)
			End If
		End Sub

		Private Shared activationMap As IDictionary(Of String, Type)
		Private Shared ReadOnly Property Map As IDictionary(Of String, [Class])
			Get
				SyncLock GetType(BaseNetConfigDeserializer)
					If activationMap Is Nothing Then
						activationMap = New Dictionary(Of String, Type)()
						For Each a As Activation In Activation.values()
							activationMap(a.ToString().ToLower()) = a.getActivationFunction().GetType()
						Next a
					End If
					Return activationMap
				End SyncLock
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void resolve(org.nd4j.shade.jackson.databind.DeserializationContext ctxt) throws org.nd4j.shade.jackson.databind.JsonMappingException
		Public Overrides Sub resolve(ByVal ctxt As DeserializationContext)
			CType(defaultDeserializer, ResolvableDeserializer).resolve(ctxt)
		End Sub
	End Class

End Namespace