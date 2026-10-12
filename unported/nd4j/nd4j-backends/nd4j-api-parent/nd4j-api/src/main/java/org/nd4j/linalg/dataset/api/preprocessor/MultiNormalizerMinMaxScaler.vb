Imports System
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports NormalizerType = org.nd4j.linalg.dataset.api.preprocessor.serializer.NormalizerType
Imports MinMaxStats = org.nd4j.linalg.dataset.api.preprocessor.stats.MinMaxStats
Imports NormalizerStats = org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats

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

Namespace org.nd4j.linalg.dataset.api.preprocessor

	<Serializable>
	Public Class MultiNormalizerMinMaxScaler
		Inherits AbstractMultiDataSetNormalizer(Of MinMaxStats)

		Public Sub New()
			Me.New(0.0, 1.0)
		End Sub

		''' <summary>
		''' Preprocessor can take a range as minRange and maxRange
		''' </summary>
		''' <param name="minRange"> the target range lower bound </param>
		''' <param name="maxRange"> the target range upper bound </param>
		Public Sub New(ByVal minRange As Double, ByVal maxRange As Double)
			MyBase.New(New MinMaxStrategy(minRange, maxRange))
		End Sub

		Public Overridable ReadOnly Property TargetMin As Double
			Get
				Return CType(strategy, MinMaxStrategy).getMinRange()
			End Get
		End Property

		Public Overridable ReadOnly Property TargetMax As Double
			Get
				Return CType(strategy, MinMaxStrategy).getMaxRange()
			End Get
		End Property

		Protected Friend Overrides Function newBuilder() As NormalizerStats.Builder
			Return New MinMaxStats.Builder()
		End Function

		Public Overridable Function getMin(ByVal input As Integer) As INDArray
			Return getFeatureStats(input).getLower()
		End Function

		Public Overridable Function getMax(ByVal input As Integer) As INDArray
			Return getFeatureStats(input).getUpper()
		End Function

		Public Overridable Function getLabelMin(ByVal output As Integer) As INDArray
			Return getLabelStats(output).getLower()
		End Function

		Public Overridable Function getLabelMax(ByVal output As Integer) As INDArray
			Return getLabelStats(output).getUpper()
		End Function

		Public Overridable Function [getType]() As NormalizerType
			Return NormalizerType.MULTI_MIN_MAX
		End Function
	End Class

End Namespace