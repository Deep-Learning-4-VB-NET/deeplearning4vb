Imports System
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet

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

Namespace org.deeplearning4j.spark.parameterserver.python


	<Serializable>
	Public Class DataSetDescriptor
'JAVA TO VB CONVERTER NOTE: The field features was renamed since Visual Basic does not allow fields to have the same name as other class members:
'JAVA TO VB CONVERTER NOTE: The field labels was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private features_Conflict, labels_Conflict As ArrayDescriptor
'JAVA TO VB CONVERTER NOTE: The field featuresMask was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private featuresMask_Conflict As ArrayDescriptor
'JAVA TO VB CONVERTER NOTE: The field labelsMask was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private labelsMask_Conflict As ArrayDescriptor
		Private preProcessed As Boolean

		Public Sub New(ByVal features As ArrayDescriptor, ByVal labels As ArrayDescriptor, ByVal featuresMask As ArrayDescriptor, ByVal labelsMask As ArrayDescriptor)
			Me.features_Conflict = features
			Me.labels_Conflict = labels
			Me.featuresMask_Conflict = featuresMask
			Me.labelsMask_Conflict = labelsMask
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public DataSetDescriptor(org.nd4j.linalg.dataset.DataSet ds)throws Exception
		Public Sub New(ByVal ds As DataSet)
			features_Conflict = New ArrayDescriptor(ds.Features)
			labels_Conflict = New ArrayDescriptor(ds.Labels)
			Dim featuresMask As INDArray = ds.FeaturesMaskArray
			If featuresMask Is Nothing Then
				Me.featuresMask_Conflict = Nothing
			Else
				Me.featuresMask_Conflict = New ArrayDescriptor(featuresMask)
			End If
			Dim labelsMask As INDArray = ds.LabelsMaskArray
			If labelsMask Is Nothing Then
				Me.labelsMask_Conflict = Nothing
			Else
				Me.labelsMask_Conflict = New ArrayDescriptor(labelsMask)
			End If

			preProcessed = ds.PreProcessed
		End Sub

		Public Overridable ReadOnly Property DataSet As DataSet
			Get
				Dim features As INDArray = Me.features_Conflict.Array
				Dim labels As INDArray = Me.labels_Conflict.Array
				Dim featuresMask As INDArray
				Dim labelsMask As INDArray
				If Me.featuresMask_Conflict Is Nothing Then
					featuresMask = Nothing
				Else
					featuresMask = Me.featuresMask_Conflict.Array
				End If
				If Me.labelsMask_Conflict Is Nothing Then
					labelsMask = Nothing
				Else
					labelsMask = Me.labelsMask_Conflict.Array
				End If
				Dim ds As New DataSet(features, labels, featuresMask, labelsMask)
				If preProcessed Then
					ds.markAsPreProcessed()
				End If
				Return ds
			End Get
		End Property

		Public Overridable ReadOnly Property Features As ArrayDescriptor
			Get
				Return features_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property Labels As ArrayDescriptor
			Get
				Return labels_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property FeaturesMask As ArrayDescriptor
			Get
				Return featuresMask_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property LabelsMask As ArrayDescriptor
			Get
				Return labelsMask_Conflict
			End Get
		End Property
	End Class

End Namespace