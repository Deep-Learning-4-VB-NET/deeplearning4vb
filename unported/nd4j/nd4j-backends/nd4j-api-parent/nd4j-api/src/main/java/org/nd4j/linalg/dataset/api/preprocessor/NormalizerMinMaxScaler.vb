Imports System
Imports NonNull = lombok.NonNull
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports NormalizerSerializer = org.nd4j.linalg.dataset.api.preprocessor.serializer.NormalizerSerializer
Imports NormalizerType = org.nd4j.linalg.dataset.api.preprocessor.serializer.NormalizerType
Imports MinMaxStats = org.nd4j.linalg.dataset.api.preprocessor.stats.MinMaxStats
Imports NormalizerStats = org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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
	Public Class NormalizerMinMaxScaler
		Inherits AbstractDataSetNormalizer(Of MinMaxStats)

		Public Sub New()
			Me.New(0.0, 1.0)
		End Sub

		''' <summary>
		''' Preprocessor can take a range as minRange and maxRange
		''' </summary>
		''' <param name="minRange"> </param>
		''' <param name="maxRange"> </param>
		Public Sub New(ByVal minRange As Double, ByVal maxRange As Double)
			MyBase.New(New MinMaxStrategy(minRange, maxRange))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setFeatureStats(@NonNull INDArray featureMin, @NonNull INDArray featureMax)
		Public Overridable Sub setFeatureStats(ByVal featureMin As INDArray, ByVal featureMax As INDArray)
			setFeatureStats(New MinMaxStats(featureMin, featureMax))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setLabelStats(@NonNull INDArray labelMin, @NonNull INDArray labelMax)
		Public Overridable Sub setLabelStats(ByVal labelMin As INDArray, ByVal labelMax As INDArray)
			setLabelStats(New MinMaxStats(labelMin, labelMax))
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

		Public Overridable ReadOnly Property Min As INDArray
			Get
				Return FeatureStats.getLower()
			End Get
		End Property

		Public Overridable ReadOnly Property Max As INDArray
			Get
				Return FeatureStats.getUpper()
			End Get
		End Property

		Public Overridable ReadOnly Property LabelMin As INDArray
			Get
				Return LabelStats.getLower()
			End Get
		End Property

		Public Overridable ReadOnly Property LabelMax As INDArray
			Get
				Return LabelStats.getUpper()
			End Get
		End Property

		''' <summary>
		''' Load the given min and max
		''' </summary>
		''' <param name="statistics"> the statistics to load </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void load(java.io.File... statistics) throws java.io.IOException
		Public Overridable Sub load(ParamArray ByVal statistics() As File)
			setFeatureStats(New MinMaxStats(Nd4j.readBinary(statistics(0)), Nd4j.readBinary(statistics(1))))
			If FitLabel Then
				setLabelStats(New MinMaxStats(Nd4j.readBinary(statistics(2)), Nd4j.readBinary(statistics(3))))
			End If
		End Sub

		''' <summary>
		''' Save the current min and max
		''' </summary>
		''' <param name="files"> the statistics to save </param>
		''' <exception cref="IOException"> </exception>
		''' @deprecated use <seealso cref="NormalizerSerializer instead"/> 
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(java.io.File... files) throws java.io.IOException
		Public Overridable Sub save(ParamArray ByVal files() As File)
			Nd4j.saveBinary(Min, files(0))
			Nd4j.saveBinary(Max, files(1))
			If FitLabel Then
				Nd4j.saveBinary(LabelMin, files(2))
				Nd4j.saveBinary(LabelMax, files(3))
			End If
		End Sub

		Protected Friend Overrides Function newBuilder() As NormalizerStats.Builder
			Return New MinMaxStats.Builder()
		End Function

		Public Overridable Function [getType]() As NormalizerType
			Return NormalizerType.MIN_MAX
		End Function
	End Class

End Namespace