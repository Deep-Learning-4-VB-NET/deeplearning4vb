Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports Lists = org.nd4j.shade.guava.collect.Lists
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetUtil = org.nd4j.linalg.dataset.api.DataSetUtil
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports NormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports FeatureUtil = org.nd4j.linalg.util.FeatureUtil
Imports MathUtils = org.nd4j.common.util.MathUtils
import static org.nd4j.linalg.indexing.NDArrayIndex.all
import static org.nd4j.linalg.indexing.NDArrayIndex.interval

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

Namespace org.nd4j.linalg.dataset



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DataSet implements org.nd4j.linalg.dataset.api.DataSet
	Public Class DataSet
		Implements org.nd4j.linalg.dataset.api.DataSet

		Private Const serialVersionUID As Long = 1935520764586513365L

		Private Const BITMASK_FEATURES_PRESENT As SByte = 1
		Private Shared ReadOnly BITMASK_LABELS_PRESENT As SByte = 1 << 1
		Private Shared ReadOnly BITMASK_LABELS_SAME_AS_FEATURES As SByte = 1 << 2
		Private Shared ReadOnly BITMASK_FEATURE_MASK_PRESENT As SByte = 1 << 3
		Private Shared ReadOnly BITMASK_LABELS_MASK_PRESENT As SByte = 1 << 4
		Private Shared ReadOnly BITMASK_METADATA_PRESET As SByte = 1 << 5

'JAVA TO VB CONVERTER NOTE: The field columnNames was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private columnNames_Conflict As IList(Of String) = New List(Of String)()
'JAVA TO VB CONVERTER NOTE: The field labelNames was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private labelNames_Conflict As IList(Of String) = New List(Of String)()
'JAVA TO VB CONVERTER NOTE: The field features was renamed since Visual Basic does not allow fields to have the same name as other class members:
'JAVA TO VB CONVERTER NOTE: The field labels was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private features_Conflict, labels_Conflict As INDArray
		Private featuresMask As INDArray
		Private labelsMask As INDArray

'JAVA TO VB CONVERTER NOTE: The field exampleMetaData was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private exampleMetaData_Conflict As IList(Of Serializable)

'JAVA TO VB CONVERTER NOTE: The field preProcessed was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Private preProcessed_Conflict As Boolean = False

		Public Sub New()
			Me.New(Nothing, Nothing)
		End Sub

		Public Overridable Property ExampleMetaData As IList(Of Serializable)
			Get
				Return exampleMetaData_Conflict
			End Get
			Set(ByVal exampleMetaData As IList(Of T1))
				Me.exampleMetaData_Conflict = CType(exampleMetaData, IList(Of Serializable))
			End Set
		End Property

		Public Overridable Function getExampleMetaData(Of T As Serializable)(ByVal metaDataType As Type(Of T)) As IList(Of T)
			Return CType(exampleMetaData_Conflict, IList(Of T))
		End Function



		''' <summary>
		''' Creates a dataset with the specified input matrix and labels
		''' </summary>
		''' <param name="first">  the feature matrix </param>
		''' <param name="second"> the labels (these should be binarized label matrices such that the specified label
		'''               has a value of 1 in the desired column with the label) </param>
		Public Sub New(ByVal first As INDArray, ByVal second As INDArray)
			Me.New(first, second, Nothing, Nothing)
		End Sub

		''' <summary>
		'''Create a dataset with the specified input INDArray and labels (output) INDArray, plus (optionally) mask arrays
		''' for the features and labels </summary>
		''' <param name="features"> Features (input) </param>
		''' <param name="labels"> Labels (output) </param>
		''' <param name="featuresMask"> Mask array for features, may be null </param>
		''' <param name="labelsMask"> Mask array for labels, may be null </param>
		Public Sub New(ByVal features As INDArray, ByVal labels As INDArray, ByVal featuresMask As INDArray, ByVal labelsMask As INDArray)
			Me.features_Conflict = features
			Me.labels_Conflict = labels
			Me.featuresMask = featuresMask
			Me.labelsMask = labelsMask

			' we want this dataset to be fully committed to device
			Nd4j.Executioner.commit()
		End Sub

		Public Overridable ReadOnly Property PreProcessed As Boolean
			Get
				Return preProcessed_Conflict
			End Get
		End Property

		Public Overridable Sub markAsPreProcessed()
			Me.preProcessed_Conflict = True
		End Sub

		''' <summary>
		''' Returns a single dataset (all fields are null)
		''' </summary>
		''' <returns> an empty dataset (all fields are null) </returns>
		Public Shared Function empty() As DataSet
			Return New DataSet(Nothing, Nothing)
		End Function

		''' <summary>
		''' Merge the list of datasets in to one list.
		''' All the rows are merged in to one dataset
		''' </summary>
		''' <param name="data"> the data to merge </param>
		''' <returns> a single dataset </returns>
		Public Shared Function merge(Of T1 As org.nd4j.linalg.dataset.api.DataSet)(ByVal data As IList(Of T1)) As DataSet
			If data.Count = 0 Then
				Throw New System.ArgumentException("Unable to merge empty dataset")
			End If

			Dim nonEmpty As Integer = 0
			Dim anyFeaturesPreset As Boolean = False
			Dim anyLabelsPreset As Boolean = False
			Dim first As Boolean = True
			For Each ds As org.nd4j.linalg.dataset.api.DataSet In data
				If ds.Empty Then
					Continue For
				End If
				nonEmpty += 1

				If anyFeaturesPreset AndAlso ds.Features Is Nothing OrElse (Not first AndAlso Not anyFeaturesPreset AndAlso ds.Features IsNot Nothing) Then
					Throw New System.InvalidOperationException("Cannot merge features: encountered null features in one or more DataSets")
				End If
				If anyLabelsPreset AndAlso ds.Labels Is Nothing OrElse (Not first AndAlso Not anyLabelsPreset AndAlso ds.Labels IsNot Nothing) Then
					Throw New System.InvalidOperationException("Cannot merge labels: enountered null labels in one or more DataSets")
				End If

				anyFeaturesPreset = anyFeaturesPreset Or ds.Features IsNot Nothing
				anyLabelsPreset = anyLabelsPreset Or ds.Labels IsNot Nothing
				first = False
			Next ds

			Dim featuresToMerge(nonEmpty - 1) As INDArray
			Dim labelsToMerge(nonEmpty - 1) As INDArray
			Dim featuresMasksToMerge() As INDArray = Nothing
			Dim labelsMasksToMerge() As INDArray = Nothing
			Dim count As Integer = 0
			For Each ds As org.nd4j.linalg.dataset.api.DataSet In data
				If ds.Empty Then
					Continue For
				End If
				featuresToMerge(count) = ds.Features
				labelsToMerge(count) = ds.Labels

				If ds.FeaturesMaskArray IsNot Nothing Then
					If featuresMasksToMerge Is Nothing Then
						featuresMasksToMerge = New INDArray(nonEmpty - 1){}
					End If
					featuresMasksToMerge(count) = ds.FeaturesMaskArray
				End If
				If ds.LabelsMaskArray IsNot Nothing Then
					If labelsMasksToMerge Is Nothing Then
						labelsMasksToMerge = New INDArray(nonEmpty - 1){}
					End If
					labelsMasksToMerge(count) = ds.LabelsMaskArray
				End If

				count += 1
			Next ds

			Dim featuresOut As INDArray
			Dim labelsOut As INDArray
			Dim featuresMaskOut As INDArray
			Dim labelsMaskOut As INDArray

			Dim fp As Pair(Of INDArray, INDArray) = DataSetUtil.mergeFeatures(featuresToMerge, featuresMasksToMerge)
			featuresOut = fp.First
			featuresMaskOut = fp.Second

			Dim lp As Pair(Of INDArray, INDArray) = DataSetUtil.mergeLabels(labelsToMerge, labelsMasksToMerge)
			labelsOut = lp.First
			labelsMaskOut = lp.Second

			Dim dataset As New DataSet(featuresOut, labelsOut, featuresMaskOut, labelsMaskOut)

			Dim meta As IList(Of Serializable) = Nothing
			For Each ds As org.nd4j.linalg.dataset.api.DataSet In data
				If ds.getExampleMetaData() Is Nothing OrElse ds.getExampleMetaData().Count <> ds.numExamples() Then
					meta = Nothing
					Exit For
				End If
				If meta Is Nothing Then
					meta = New List(Of Serializable)()
				End If
				CType(meta, List(Of Serializable)).AddRange(ds.getExampleMetaData())
			Next ds
			If meta IsNot Nothing Then
				dataset.ExampleMetaData = meta
			End If

			Return dataset
		End Function

		Public Overridable Function getRange(ByVal from As Integer, ByVal [to] As Integer) As org.nd4j.linalg.dataset.api.DataSet
			If hasMaskArrays() Then
				Dim featureMaskHere As INDArray = If(featuresMask IsNot Nothing, featuresMask.get(interval(from, [to])), Nothing)
				Dim labelMaskHere As INDArray = If(labelsMask IsNot Nothing, labelsMask.get(interval(from, [to])), Nothing)
				Return New DataSet(features_Conflict.get(interval(from, [to])), labels_Conflict.get(interval(from, [to])), featureMaskHere, labelMaskHere)
			End If
			Return New DataSet(features_Conflict.get(interval(from, [to])), labels_Conflict.get(interval(from, [to])))
		End Function


		Public Overridable Sub load(ByVal from As Stream)
			Try

				Dim dis As DataInputStream = If(TypeOf from Is BufferedInputStream, New DataInputStream(from), New DataInputStream(New BufferedInputStream(from)))

				Dim included As SByte = dis.readByte()
				Dim hasFeatures As Boolean = (included And BITMASK_FEATURES_PRESENT) <> 0
				Dim hasLabels As Boolean = (included And BITMASK_LABELS_PRESENT) <> 0
				Dim hasLabelsSameAsFeatures As Boolean = (included And BITMASK_LABELS_SAME_AS_FEATURES) <> 0
				Dim hasFeaturesMask As Boolean = (included And BITMASK_FEATURE_MASK_PRESENT) <> 0
				Dim hasLabelsMask As Boolean = (included And BITMASK_LABELS_MASK_PRESENT) <> 0
				Dim hasMetaData As Boolean = (included And BITMASK_METADATA_PRESET) <> 0

				features_Conflict = (If(hasFeatures, Nd4j.read(dis), Nothing))
				If hasLabels Then
					labels_Conflict = Nd4j.read(dis)
				ElseIf hasLabelsSameAsFeatures Then
					labels_Conflict = features_Conflict
				Else
					labels_Conflict = Nothing
				End If

				featuresMask = (If(hasFeaturesMask, Nd4j.read(dis), Nothing))
				labelsMask = (If(hasLabelsMask, Nd4j.read(dis), Nothing))

				If hasMetaData Then
					Dim ois As New ObjectInputStream(dis)
					exampleMetaData_Conflict = CType(ois.readObject(), IList(Of Serializable))
				End If

				dis.close()
			Catch e As Exception
				Throw New Exception("Error loading DataSet",e)
			End Try
		End Sub

		Public Overridable Sub load(ByVal from As File)
			Try
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: Using fis As System.IO.FileStream_Input = new System.IO.FileStream(from, System.IO.FileMode.Open, System.IO.FileAccess.Read), bis As BufferedInputStream = new BufferedInputStream(fis, 1024 * 1024)
					New FileStream(from, FileMode.Open, FileAccess.Read), bis As New BufferedInputStream(fis, 1024 * 1024)
						Using fis As New FileStream(from, FileMode.Open, FileAccess.Read), bis As BufferedInputStream
					load(bis)
					End Using
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Sub


		Public Overridable Sub save(ByVal [to] As Stream)

			Dim included As SByte = 0
			If features_Conflict IsNot Nothing Then
				included = included Or BITMASK_FEATURES_PRESENT
			End If
			If labels_Conflict IsNot Nothing Then
				If labels_Conflict Is features_Conflict Then
					'Same object. Don't serialize the same data twice!
					included = included Or BITMASK_LABELS_SAME_AS_FEATURES
				Else
					included = included Or BITMASK_LABELS_PRESENT
				End If
			End If
			If featuresMask IsNot Nothing Then
				included = included Or BITMASK_FEATURE_MASK_PRESENT
			End If
			If labelsMask IsNot Nothing Then
				included = included Or BITMASK_LABELS_MASK_PRESENT
			End If
			If exampleMetaData_Conflict IsNot Nothing AndAlso exampleMetaData_Conflict.Count > 0 Then
				included = included Or BITMASK_METADATA_PRESET
			End If


			Try
				Dim bos As New BufferedOutputStream([to])
				Dim dos As New DataOutputStream(bos)
				dos.writeByte(included)

				If features_Conflict IsNot Nothing Then
					Nd4j.write(features_Conflict, dos)
				End If
				If labels_Conflict IsNot Nothing AndAlso labels_Conflict IsNot features_Conflict Then
					Nd4j.write(labels_Conflict, dos)
				End If
				If featuresMask IsNot Nothing Then
					Nd4j.write(featuresMask, dos)
				End If
				If labelsMask IsNot Nothing Then
					Nd4j.write(labelsMask, dos)
				End If
				If exampleMetaData_Conflict IsNot Nothing AndAlso exampleMetaData_Conflict.Count > 0 Then
					Dim oos As New ObjectOutputStream(bos)
					oos.writeObject(exampleMetaData_Conflict)
					oos.flush()
				End If


				dos.flush()
				dos.close()
			Catch e As Exception
				log.error("",e)
			End Try
		End Sub

		Public Overridable Sub save(ByVal [to] As File)
			Try
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: Using fos As System.IO.FileStream_Output = new System.IO.FileStream_Output(to, false), bos As BufferedOutputStream = new BufferedOutputStream(fos)
					New FileStream([to], False), bos As New BufferedOutputStream(fos)
						Using fos As New FileStream([to], False), bos As BufferedOutputStream
					save(bos)
					End Using
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Sub

		Public Overridable Function iterateWithMiniBatches() As DataSetIterator Implements org.nd4j.linalg.dataset.api.DataSet.iterateWithMiniBatches
			Return Nothing
		End Function

		Public Overridable Function id() As String Implements org.nd4j.linalg.dataset.api.DataSet.id
			Return ""
		End Function

		Public Overridable Property Features As INDArray Implements org.nd4j.linalg.dataset.api.DataSet.getFeatures
			Get
				Return features_Conflict
			End Get
			Set(ByVal features As INDArray)
				Me.features_Conflict = features
			End Set
		End Property


		Public Overridable Function labelCounts() As IDictionary(Of Integer, Double)
			Dim ret As IDictionary(Of Integer, Double) = New Dictionary(Of Integer, Double)()
			If labels_Conflict Is Nothing Then
				Return ret
			End If
			Dim nTensors As Long = labels_Conflict.tensorsAlongDimension(1)
			For i As Integer = 0 To nTensors - 1
				Dim row As INDArray = labels_Conflict.tensorAlongDimension(i, 1)
				Dim javaRow As INDArray = labels_Conflict.tensorAlongDimension(i, 1)
				Dim maxIdx As Integer = Nd4j.BlasWrapper.iamax(row)
				Dim maxIdxJava As Integer = Nd4j.BlasWrapper.iamax(javaRow)
				If maxIdx < 0 Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.InvalidOperationException("Please check the iamax implementation for " & Nd4j.BlasWrapper.GetType().FullName)
				End If
				If ret(maxIdx) = Nothing Then
					ret(maxIdx) = 1.0
				Else
					ret(maxIdx) = ret(maxIdx) + 1.0
				End If
			Next i
			Return ret
		End Function

		''' <summary>
		''' Clone the dataset
		''' </summary>
		''' <returns> a clone of the dataset </returns>
		Public Overridable Function copy() As DataSet
			Dim ret As New DataSet(Features.dup(), Labels.dup())
			If LabelsMaskArray IsNot Nothing Then
				ret.LabelsMaskArray = LabelsMaskArray.dup()
			End If
			If FeaturesMaskArray IsNot Nothing Then
				ret.FeaturesMaskArray = FeaturesMaskArray.dup()
			End If
			ret.ColumnNames = getColumnNames()
			ret.LabelNames = getLabelNames()
			Return ret
		End Function

		''' <summary>
		''' Reshapes the input in to the given rows and columns
		''' </summary>
		''' <param name="rows"> the row size </param>
		''' <param name="cols"> the column size </param>
		''' <returns> a copy of this data op with the input resized </returns>
		Public Overridable Function reshape(ByVal rows As Integer, ByVal cols As Integer) As DataSet
			Dim ret As New DataSet(Features.reshape(New Long() {rows, cols}), Labels)
			Return ret
		End Function


		Public Overridable Sub multiplyBy(ByVal num As Double) Implements org.nd4j.linalg.dataset.api.DataSet.multiplyBy
			Features.muli(Nd4j.scalar(num))
		End Sub

		Public Overridable Sub divideBy(ByVal num As Integer) Implements org.nd4j.linalg.dataset.api.DataSet.divideBy
			Features.divi(Nd4j.scalar(num))
		End Sub

		Public Overridable Sub shuffle() Implements org.nd4j.linalg.dataset.api.DataSet.shuffle
			Dim seed As Long = DateTimeHelper.CurrentUnixTimeMillis()
			shuffle(seed)
		End Sub

		''' <summary>
		''' Shuffles the dataset in place, given a seed for a random number generator. For reproducibility
		''' This will modify the dataset in place!!
		''' </summary>
		''' <param name="seed"> Seed to use for the random Number Generator </param>
		Public Overridable Sub shuffle(ByVal seed As Long)
			' just skip shuffle if there's only 1 example
			If numExamples() < 2 Then
				Return
			End If

			'note here we use the same seed with different random objects guaranteeing same order

			Dim arrays As IList(Of INDArray) = New List(Of INDArray)()
			Dim dimensions As IList(Of Integer()) = New List(Of Integer())()

			arrays.Add(Features)
			dimensions.Add(ArrayUtil.range(1, Features.rank()))

			arrays.Add(Labels)
			dimensions.Add(ArrayUtil.range(1, Labels.rank()))

			If featuresMask IsNot Nothing Then
				arrays.Add(FeaturesMaskArray)
				dimensions.Add(ArrayUtil.range(1, FeaturesMaskArray.rank()))
			End If

			If labelsMask IsNot Nothing Then
				arrays.Add(LabelsMaskArray)
				dimensions.Add(ArrayUtil.range(1, LabelsMaskArray.rank()))
			End If

			Nd4j.shuffle(arrays, New Random(seed), dimensions)

			'As per CpuNDArrayFactory.shuffle(List<INDArray> arrays, Random rnd, List<int[]> dimensions) and libnd4j transforms.h shuffleKernelGeneric
			If exampleMetaData_Conflict IsNot Nothing Then
				Dim map() As Integer = ArrayUtil.buildInterleavedVector(New Random(seed), numExamples())
				ArrayUtil.shuffleWithMap(exampleMetaData_Conflict, map)
			End If
		End Sub


		''' <summary>
		''' Squeezes input data to a max and a min
		''' </summary>
		''' <param name="min"> the min value to occur in the dataset </param>
		''' <param name="max"> the max value to ccur in the dataset </param>
		Public Overridable Sub squishToRange(ByVal min As Double, ByVal max As Double) Implements org.nd4j.linalg.dataset.api.DataSet.squishToRange
			Dim i As Integer = 0
			Do While i < Features.length()
				Dim curr As Double = DirectCast(Features.getScalar(i).element(), Double)
				If curr < min Then
					Features.put(i, Nd4j.scalar(min))
				ElseIf curr > max Then
					Features.put(i, Nd4j.scalar(max))
				End If
				i += 1
			Loop
		End Sub

		Public Overridable Sub scaleMinAndMax(ByVal min As Double, ByVal max As Double) Implements org.nd4j.linalg.dataset.api.DataSet.scaleMinAndMax
			FeatureUtil.scaleMinMax(min, max, Features)
		End Sub

		''' <summary>
		''' Divides the input data transform
		''' by the max number in each row
		''' </summary>
		Public Overridable Sub scale() Implements org.nd4j.linalg.dataset.api.DataSet.scale
			FeatureUtil.scaleByMax(Features)
		End Sub

		''' <summary>
		''' Adds a feature for each example on to the current feature vector
		''' </summary>
		''' <param name="toAdd"> the feature vector to add </param>
		Public Overridable Sub addFeatureVector(ByVal toAdd As INDArray) Implements org.nd4j.linalg.dataset.api.DataSet.addFeatureVector
			Features = Nd4j.hstack(Features, toAdd)
		End Sub


		''' <summary>
		''' The feature to add, and the example/row number
		''' </summary>
		''' <param name="feature"> the feature vector to add </param>
		''' <param name="example"> the number of the example to append to </param>
		Public Overridable Sub addFeatureVector(ByVal feature As INDArray, ByVal example As Integer) Implements org.nd4j.linalg.dataset.api.DataSet.addFeatureVector
			Features.putRow(example, feature)
		End Sub

		Public Overridable Sub normalize() Implements org.nd4j.linalg.dataset.api.DataSet.normalize
			'FeatureUtil.normalizeMatrix(getFeatures());
			Dim inClassPreProcessor As New NormalizerStandardize()
			inClassPreProcessor.fit(Me)
			inClassPreProcessor.transform(Me)
		End Sub


		''' <summary>
		''' Same as calling binarize(0)
		''' </summary>
		Public Overridable Sub binarize() Implements org.nd4j.linalg.dataset.api.DataSet.binarize
			binarize(0)
		End Sub

		''' <summary>
		''' Binarizes the dataset such that any number greater than cutoff is 1 otherwise zero
		''' </summary>
		''' <param name="cutoff"> the cutoff point </param>
		Public Overridable Sub binarize(ByVal cutoff As Double) Implements org.nd4j.linalg.dataset.api.DataSet.binarize
			Dim linear As INDArray = Features.reshape(ChrW(-1))
			Dim i As Integer = 0
			Do While i < Features.length()
				Dim curr As Double = linear.getDouble(i)
				If curr > cutoff Then
					Features.putScalar(i, 1)
				Else
					Features.putScalar(i, 0)
				End If
				i += 1
			Loop
		End Sub


		''' <summary>
		''' @Deprecated
		''' Subtract by the column means and divide by the standard deviation
		''' </summary>
		<Obsolete>
		Public Overridable Sub normalizeZeroMeanZeroUnitVariance() Implements org.nd4j.linalg.dataset.api.DataSet.normalizeZeroMeanZeroUnitVariance
			Dim columnMeans As INDArray = Features.mean(0)
			Dim columnStds As INDArray = Features.std(0)

			Features = Features.subiRowVector(columnMeans)
			columnStds.addi(Nd4j.scalar(Nd4j.EPS_THRESHOLD))
			Features = Features.diviRowVector(columnStds)
		End Sub

		''' <summary>
		''' The number of inputs in the feature matrix
		''' 
		''' @return
		''' </summary>
		Public Overridable Function numInputs() As Integer Implements org.nd4j.linalg.dataset.api.DataSet.numInputs
			Return CInt(Features.size(1))
		End Function

		Public Overridable Sub validate() Implements org.nd4j.linalg.dataset.api.DataSet.validate
			If Features.size(0) <> Labels.size(0) Then
				Throw New System.InvalidOperationException("Invalid dataset")
			End If
		End Sub

		Public Overridable Function outcome() As Integer Implements org.nd4j.linalg.dataset.api.DataSet.outcome
			Return Nd4j.BlasWrapper.iamax(Labels)
		End Function

		''' <summary>
		''' Clears the outcome matrix setting a new number of labels
		''' </summary>
		''' <param name="labels"> the number of labels/columns in the outcome matrix
		'''               Note that this clears the labels for each example </param>
		Public Overridable WriteOnly Property NewNumberOfLabels Implements org.nd4j.linalg.dataset.api.DataSet.setNewNumberOfLabels As Integer
			Set(ByVal labels As Integer)
				Dim examples As Integer = numExamples()
				Dim newOutcomes As INDArray = Nd4j.create(examples, labels)
				Me.Labels = newOutcomes
			End Set
		End Property

		''' <summary>
		''' Sets the outcome of a particular example
		''' </summary>
		''' <param name="example"> the example to transform </param>
		''' <param name="label">   the label of the outcome </param>
		Public Overridable Sub setOutcome(ByVal example As Integer, ByVal label As Integer) Implements org.nd4j.linalg.dataset.api.DataSet.setOutcome
			If example > numExamples() Then
				Throw New System.ArgumentException("No example at " & example)
			End If
			If label > numOutcomes() OrElse label < 0 Then
				Throw New System.ArgumentException("Illegal label")
			End If

			Dim outcome As INDArray = FeatureUtil.toOutcomeVector(label, numOutcomes())
			Labels.putRow(example, outcome)
		End Sub

		''' <summary>
		''' Gets a copy of example i
		''' </summary>
		''' <param name="i"> the example to getFromOrigin </param>
		''' <returns> the example at i (one example) </returns>
		Public Overridable Function get(ByVal i As Integer) As DataSet
			If i >= numExamples() OrElse i < 0 Then
				Throw New System.ArgumentException("invalid example number: must be 0 to " & (numExamples()-1) & ", got " & i)
			End If
			If i = 0 AndAlso numExamples() = 1 Then
				Return Me
			End If
			Return New DataSet(getHelper(features_Conflict,i), getHelper(labels_Conflict, i), getHelper(featuresMask,i), getHelper(labelsMask,i))
		End Function

		''' <summary>
		''' Gets a copy of example i
		''' </summary>
		''' <param name="i"> the example to getFromOrigin </param>
		''' <returns> the example at i (one example) </returns>
		Public Overridable Function get(ByVal i() As Integer) As DataSet
			Dim list As IList(Of DataSet) = New List(Of DataSet)()
			For Each ex As Integer In i
				list.Add(get(ex))
			Next ex
			Return DataSet.merge(list)
		End Function

		''' <summary>
		''' Partitions a dataset in to mini batches where
		''' each dataset in each list is of the specified number of examples
		''' </summary>
		''' <param name="num"> the number to split by </param>
		''' <returns> the partitioned datasets </returns>
		Public Overridable Function batchBy(ByVal num As Integer) As IList(Of DataSet)
			Dim batched As IList(Of DataSet) = Lists.newArrayList()
			For Each splitBatch As IList(Of DataSet) In Lists.partition(asList(), num)
				batched.Add(DataSet.merge(splitBatch))
			Next splitBatch
			Return batched
		End Function

		''' <summary>
		''' Strips the data transform of all but the passed in labels
		''' </summary>
		''' <param name="labels"> strips the data transform of all but the passed in labels </param>
		''' <returns> the dataset with only the specified labels </returns>
		Public Overridable Function filterBy(ByVal labels() As Integer) As DataSet
			Dim list As IList(Of DataSet) = New List(Of DataSet) From {}
			Dim newList As IList(Of DataSet) = New List(Of DataSet)()
			Dim labelList As IList(Of Integer) = New List(Of Integer)()
			For Each i As Integer In labels
				labelList.Add(i)
			Next i
			For Each d As DataSet In list
				Dim outcome As Integer = d.outcome()
				If labelList.Contains(outcome) Then
					newList.Add(d)
				End If
			Next d

			Return DataSet.merge(newList)
		End Function

		''' <summary>
		''' Strips the dataset down to the specified labels
		''' and remaps them
		''' </summary>
		''' <param name="labels"> the labels to strip down to </param>
		Public Overridable Sub filterAndStrip(ByVal labels() As Integer) Implements org.nd4j.linalg.dataset.api.DataSet.filterAndStrip
			Dim filtered As DataSet = filterBy(labels)
			Dim newLabels As IList(Of Integer) = New List(Of Integer)()

			'map new labels to index according to passed in labels
			Dim labelMap As IDictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()

			For i As Integer = 0 To labels.Length - 1
				labelMap(labels(i)) = i
			Next i

			'map examples
			Dim i As Integer = 0
			Do While i < filtered.numExamples()
				Dim example As DataSet = filtered.get(i)
				Dim o2 As Integer = example.outcome()
				Dim outcome As Integer? = labelMap(o2)
				newLabels.Add(outcome)

				i += 1
			Loop


			Dim newLabelMatrix As INDArray = Nd4j.create(filtered.numExamples(), labels.Length)

			If newLabelMatrix.rows() <> newLabels.Count Then
				Throw New System.InvalidOperationException("Inconsistent label sizes")
			End If

			i = 0
			Do While i < newLabelMatrix.rows()
				Dim i2 As Integer? = newLabels(i)
				If i2 Is Nothing Then
					Throw New System.InvalidOperationException("Label not found on row " & i)
				End If
				Dim newRow As INDArray = FeatureUtil.toOutcomeVector(i2, labels.Length)
				newLabelMatrix.putRow(i, newRow)

				i += 1
			Loop

			Features = filtered.Features
			Me.Labels = newLabelMatrix
		End Sub

		''' <summary>
		''' Partitions the data transform by the specified number.
		''' </summary>
		''' <param name="num"> the number to split by </param>
		''' <returns> the partitioned data transform </returns>
		Public Overridable Function dataSetBatches(ByVal num As Integer) As IList(Of DataSet)
			Dim list As IList(Of IList(Of DataSet)) = Lists.partition(asList(), num)
			Dim ret As IList(Of DataSet) = New List(Of DataSet)()
			For Each l As IList(Of DataSet) In list
				ret.Add(DataSet.merge(l))
			Next l
			Return ret

		End Function

		''' <summary>
		''' Sorts the dataset by label:
		''' Splits the data transform such that examples are sorted by their labels.
		''' A ten label dataset would produce lists with batches like the following:
		''' x1   y = 1
		''' x2   y = 2
		''' ...
		''' x10  y = 10
		''' </summary>
		''' <returns> a list of data sets partitioned by outcomes </returns>
		Public Overridable Function sortAndBatchByNumLabels() As IList(Of DataSet)
			sortByLabel()
			Return batchByNumLabels()
		End Function

		Public Overridable Function batchByNumLabels() As IList(Of DataSet)
			Return batchBy(numOutcomes())
		End Function

		Public Overridable Function asList() As IList(Of DataSet)
			Dim list As IList(Of DataSet) = New List(Of DataSet)(numExamples())
			Dim featuresHere, labelsHere, featureMaskHere, labelMaskHere As INDArray
			Dim rank As Integer = Features.rank()
			Dim labelsRank As Integer = Labels.rank()

			' Preserving the dimension of the dataset - essentially a minibatch size of 1
			Dim i As Integer = 0
			Do While i < numExamples()
				featuresHere = getHelper(Features, i)
				featureMaskHere = getHelper(featuresMask, i)
				labelsHere = getHelper(labels_Conflict, i)
				labelMaskHere = getHelper(labelsMask, i)

				Dim ds As New DataSet(featuresHere, labelsHere, featureMaskHere, labelMaskHere)
				If exampleMetaData_Conflict IsNot Nothing AndAlso exampleMetaData_Conflict.Count > i Then
					ds.ExampleMetaData = Collections.singletonList(exampleMetaData(i))
				End If
				list.Add(ds)
				i += 1
			Loop
			Return list
		End Function

		Private Function getHelper(ByVal from As INDArray, ByVal i As Integer) As INDArray
			If from Is Nothing Then
				Return Nothing
			End If
			Select Case from.rank()
				Case 2
					Return from.get(interval(i, i, True), all())
				Case 3
					Return from.get(interval(i, i, True), all(), all())
				Case 4
					Return from.get(interval(i, i, True), all(), all(), all())
				Case 5
					Return from.get(interval(i, i, True), all(), all(), all(), all())
				Case Else
					Throw New System.InvalidOperationException("Cannot convert to list: feature set rank must be in range 2 to 5 inclusive. Got shape: " & java.util.Arrays.toString(from.shape()))
			End Select
		End Function

		''' <summary>
		''' Splits a dataset in to test and train randomly.
		''' This will modify the dataset in place to shuffle it before splitting into test/train!
		''' </summary>
		''' <param name="numHoldout"> the number to hold out for training </param>
		''' <param name="rng"> Random Number Generator to use to shuffle the dataset </param>
		''' <returns> the pair of datasets for the train test split </returns>
		Public Overridable Function splitTestAndTrain(ByVal numHoldout As Integer, ByVal rng As Random) As SplitTestAndTrain
			Dim seed As Long = rng.nextLong()
			Me.shuffle(seed)
			Return splitTestAndTrain(numHoldout)
		End Function

		''' <summary>
		''' Splits a dataset in to test and train
		''' </summary>
		''' <param name="numHoldout"> the number to hold out for training </param>
		''' <returns> the pair of datasets for the train test split </returns>
		Public Overridable Function splitTestAndTrain(ByVal numHoldout As Integer) As SplitTestAndTrain
			Dim numExamples As Integer = Me.numExamples()
			If numExamples <= 1 Then
				Throw New System.InvalidOperationException("Cannot split DataSet with <= 1 rows (data set has " & numExamples & " example)")
			End If
			If numHoldout >= numExamples Then
				Throw New System.ArgumentException("Unable to split on size equal or larger than the number of rows (# numExamples=" & numExamples & ", numHoldout=" & numHoldout & ")")
			End If
			Dim first As New DataSet()
			Dim second As New DataSet()
			Select Case features_Conflict.rank()
				Case 2
					first.Features = features_Conflict.get(interval(0, numHoldout), all())
					second.Features = features_Conflict.get(interval(numHoldout, numExamples), all())
				Case 3
					first.Features = features_Conflict.get(interval(0, numHoldout), all(), all())
					second.Features = features_Conflict.get(interval(numHoldout, numExamples), all(), all())
				Case 4
					first.Features = features_Conflict.get(interval(0, numHoldout), all(), all(), all())
					second.Features = features_Conflict.get(interval(numHoldout, numExamples), all(), all(), all())
				Case Else
					Throw New System.NotSupportedException("Features rank: " & features_Conflict.rank())
			End Select
			Select Case labels_Conflict.rank()
				Case 2
					first.Labels = labels_Conflict.get(interval(0, numHoldout), all())
					second.Labels = labels_Conflict.get(interval(numHoldout, numExamples), all())
				Case 3
					first.Labels = labels_Conflict.get(interval(0, numHoldout), all(), all())
					second.Labels = labels_Conflict.get(interval(numHoldout, numExamples), all(), all())
				Case 4
					first.Labels = labels_Conflict.get(interval(0, numHoldout), all(), all(), all())
					second.Labels = labels_Conflict.get(interval(numHoldout, numExamples), all(), all(), all())
				Case Else
					Throw New System.NotSupportedException("Labels rank: " & features_Conflict.rank())
			End Select

			If featuresMask IsNot Nothing Then
				first.FeaturesMaskArray = featuresMask.get(interval(0, numHoldout), all())
				second.FeaturesMaskArray = featuresMask.get(interval(numHoldout, numExamples), all())
			End If
			If labelsMask IsNot Nothing Then
				first.LabelsMaskArray = labelsMask.get(interval(0, numHoldout), all())
				second.LabelsMaskArray = labelsMask.get(interval(numHoldout, numExamples), all())
			End If

			If exampleMetaData_Conflict IsNot Nothing Then
				Dim meta1 As IList(Of Serializable) = New List(Of Serializable)()
				Dim meta2 As IList(Of Serializable) = New List(Of Serializable)()
				Dim i As Integer = 0
				Do While i < numHoldout AndAlso i < exampleMetaData_Conflict.Count
					meta1.Add(exampleMetaData(i))
					i += 1
				Loop
				i = numHoldout
				Do While i < numExamples AndAlso i < exampleMetaData_Conflict.Count
					meta2.Add(exampleMetaData(i))
					i += 1
				Loop
				first.ExampleMetaData = meta1
				second.ExampleMetaData = meta2
			End If
			Return New SplitTestAndTrain(first, second)
		End Function


		''' <summary>
		''' Returns the labels for the dataset
		''' </summary>
		''' <returns> the labels for the dataset </returns>
		Public Overridable Property Labels As INDArray Implements org.nd4j.linalg.dataset.api.DataSet.getLabels
			Get
				Return labels_Conflict
			End Get
			Set(ByVal labels As INDArray)
				Me.labels_Conflict = labels
			End Set
		End Property

		''' <param name="idx"> the index to pullRows the string label value out of the list if it exists </param>
		''' <returns> the label opName </returns>
		Public Overridable Function getLabelName(ByVal idx As Integer) As String Implements org.nd4j.linalg.dataset.api.DataSet.getLabelName
			If labelNames_Conflict.Count > 0 Then
				If idx < labelNames_Conflict.Count Then
					Return labelNames(idx)
				Else
					Throw New System.InvalidOperationException("Index requested is longer than the number of labels used for classification.")
				End If
			Else
				Throw New System.InvalidOperationException("Label names are not defined on this dataset. Add label names in order to use getLabelName with an id.")
			End If

		End Function

		''' <param name="idxs"> list of index to pullRows the string label value out of the list if it exists </param>
		''' <returns> the label opName </returns>
		Public Overridable Function getLabelNames(ByVal idxs As INDArray) As IList(Of String)
			Dim ret As IList(Of String) = New List(Of String)()
			For i As Integer = 0 To idxs.length() - 1
				ret.Insert(i, getLabelName(i))
			Next i
			Return ret

		End Function



		''' <summary>
		''' Organizes the dataset to minimize sampling error
		''' while still allowing efficient batching.
		''' </summary>
		Public Overridable Sub sortByLabel() Implements org.nd4j.linalg.dataset.api.DataSet.sortByLabel
			Dim map As IDictionary(Of Integer, LinkedList(Of DataSet)) = New Dictionary(Of Integer, LinkedList(Of DataSet))()
			Dim data As IList(Of DataSet) = New List(Of DataSet) From {}
			Dim numLabels As Integer = numOutcomes()
			Dim examples As Integer = numExamples()
			For Each d As DataSet In data
				Dim label As Integer = d.outcome()
				Dim q As LinkedList(Of DataSet) = map(label)
				If q Is Nothing Then
					q = New LinkedList(Of DataSet)()
					map(label) = q
				End If
				q.AddLast(d)
			Next d

			For Each label As KeyValuePair(Of Integer, LinkedList(Of DataSet)) In map.SetOfKeyValuePairs()
				log.info("Label " & label & " has " & label.Value.size() & " elements")
			Next label

			'ideal input splits: 1 of each label in each batch
			'after we run out of ideal batches: fall back to a new strategy
			Dim optimal As Boolean = True
			For i As Integer = 0 To examples - 1
				If optimal Then
					For j As Integer = 0 To numLabels - 1
						Dim q As LinkedList(Of DataSet) = map(j)
						If q Is Nothing Then
							optimal = False
							Exit For
						End If
						Dim [next] As DataSet = q.RemoveFirst()
						'add a row; go to next
						If [next] IsNot Nothing Then
							addRow([next], i)
							i += 1
						Else
							optimal = False
							Exit For
						End If
					Next j
				Else
					Dim add As DataSet = Nothing
					For Each q As LinkedList(Of DataSet) In map.Values
						If q.Count > 0 Then
							add = q.RemoveFirst()
							Exit For
						End If
					Next q

					addRow(add, i)

				End If


			Next i


		End Sub


		Public Overridable Sub addRow(ByVal d As DataSet, ByVal i As Integer)
			If i > numExamples() OrElse d Is Nothing Then
				Throw New System.ArgumentException("Invalid index for adding a row")
			End If
			Features.putRow(i, d.Features)
			Labels.putRow(i, d.Labels)
		End Sub


		Private Function getLabel(ByVal data As DataSet) As Integer
			Dim f As Single? = data.Labels.maxNumber().floatValue()
			Return f.Value
		End Function


		Public Overridable Function exampleSums() As INDArray Implements org.nd4j.linalg.dataset.api.DataSet.exampleSums
			Return Features.sum(1)
		End Function

		Public Overridable Function exampleMaxs() As INDArray Implements org.nd4j.linalg.dataset.api.DataSet.exampleMaxs
			Return Features.max(1)
		End Function

		Public Overridable Function exampleMeans() As INDArray Implements org.nd4j.linalg.dataset.api.DataSet.exampleMeans
			Return Features.mean(1)
		End Function


		''' <summary>
		''' Sample without replacement and a random rng
		''' </summary>
		''' <param name="numSamples"> the number of samples to getFromOrigin </param>
		''' <returns> a sample data transform without replacement </returns>
		Public Overridable Function sample(ByVal numSamples As Integer) As DataSet
			Return sample(numSamples, Nd4j.Random)
		End Function

		''' <summary>
		''' Sample without replacement
		''' </summary>
		''' <param name="numSamples"> the number of samples to getFromOrigin </param>
		''' <param name="rng">        the rng to use </param>
		''' <returns> the sampled dataset without replacement </returns>
		Public Overridable Function sample(ByVal numSamples As Integer, ByVal rng As org.nd4j.linalg.api.rng.Random) As DataSet
			Return sample(numSamples, rng, False)
		End Function

		''' <summary>
		''' Sample a dataset numSamples times
		''' </summary>
		''' <param name="numSamples">      the number of samples to getFromOrigin </param>
		''' <param name="withReplacement"> the rng to use </param>
		''' <returns> the sampled dataset without replacement </returns>
		Public Overridable Function sample(ByVal numSamples As Integer, ByVal withReplacement As Boolean) As DataSet
			Return sample(numSamples, Nd4j.Random, withReplacement)
		End Function

		''' <summary>
		''' Sample a dataset
		''' </summary>
		''' <param name="numSamples">      the number of samples to getFromOrigin </param>
		''' <param name="rng">             the rng to use </param>
		''' <param name="withReplacement"> whether to allow duplicates (only tracked by example row number) </param>
		''' <returns> the sample dataset </returns>
		Public Overridable Function sample(ByVal numSamples As Integer, ByVal rng As org.nd4j.linalg.api.rng.Random, ByVal withReplacement As Boolean) As DataSet
			Dim added As ISet(Of Integer) = New HashSet(Of Integer)()
			Dim toMerge As IList(Of DataSet) = New List(Of DataSet)()
			Dim terminate As Boolean = False
			Dim i As Integer = 0
			Do While i < numSamples AndAlso Not terminate
				Dim picked As Integer = rng.nextInt(numExamples())
				If Not withReplacement Then
					Do While added.Contains(picked)
						picked = rng.nextInt(numExamples())
						If added.Count = numExamples() Then
							terminate = True
							Exit Do
						End If
					Loop
				End If
				added.Add(picked)
				toMerge.Add(get(picked))
				i += 1
			Loop
			Return DataSet.merge(toMerge)
		End Function

		Public Overridable Sub roundToTheNearest(ByVal roundTo As Integer) Implements org.nd4j.linalg.dataset.api.DataSet.roundToTheNearest
			Dim i As Integer = 0
			Do While i < Features.length()
				Dim curr As Double = DirectCast(Features.getScalar(i).element(), Double)
				Features.put(i, Nd4j.scalar(MathUtils.roundDouble(curr, roundTo)))
				i += 1
			Loop
		End Sub

		Public Overridable Function numOutcomes() As Integer Implements org.nd4j.linalg.dataset.api.DataSet.numOutcomes
			Return CInt(Labels.size(1))
		End Function

		Public Overridable Function numExamples() As Integer Implements org.nd4j.linalg.dataset.api.DataSet.numExamples
			If Features IsNot Nothing Then
				Return CInt(Features.size(0))
			ElseIf Labels IsNot Nothing Then
				Return CInt(Labels.size(0))
			End If
			Return 0
		End Function


		Public Overrides Function ToString() As String
			Dim builder As New StringBuilder()
			If features_Conflict IsNot Nothing AndAlso labels_Conflict IsNot Nothing Then
				builder.Append("===========INPUT===================" & vbLf).Append(Features.ToString().replaceAll(";", vbLf)).Append(vbLf & "=================OUTPUT==================" & vbLf).Append(Labels.ToString().replaceAll(";", vbLf))
				If featuresMask IsNot Nothing Then
					builder.Append(vbLf & "===========INPUT MASK===================" & vbLf).Append(FeaturesMaskArray.ToString().replaceAll(";", vbLf))
				End If
				If labelsMask IsNot Nothing Then
					builder.Append(vbLf & "===========OUTPUT MASK===================" & vbLf).Append(LabelsMaskArray.ToString().replaceAll(";", vbLf))
				End If
				Return builder.ToString()
			Else
				log.info("Features or labels are null values")
				Return ""
			End If
		End Function



		''' <summary>
		''' Gets the optional label names
		''' 
		''' @return
		''' </summary>
		<Obsolete>
		Public Overridable Property LabelNames As IList(Of String)
			Get
				Return labelNames_Conflict
			End Get
			Set(ByVal labelNames As IList(Of String))
				Me.labelNames_Conflict = labelNames
			End Set
		End Property


		''' <summary>
		''' Gets the optional label names
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property LabelNamesList As IList(Of String)
			Get
				Return labelNames_Conflict
			End Get
		End Property


		''' <summary>
		''' Optional column names of the data transform, this is mainly used
		''' for interpeting what columns are in the dataset
		''' 
		''' @return
		''' </summary>
		<Obsolete>
		Public Overridable Property ColumnNames As IList(Of String)
			Get
				Return columnNames_Conflict
			End Get
			Set(ByVal columnNames As IList(Of String))
				Me.columnNames_Conflict = columnNames
			End Set
		End Property


		Public Overridable Function splitTestAndTrain(ByVal fractionTrain As Double) As SplitTestAndTrain
			Preconditions.checkArgument(fractionTrain > 0.0 AndAlso fractionTrain < 1.0, "Train fraction must be > 0.0 and < 1.0 - got %s", fractionTrain)
			Dim numTrain As Integer = CInt(Math.Truncate(fractionTrain * numExamples()))
			If numTrain <= 0 Then
				numTrain = 1
			End If
			Return splitTestAndTrain(numTrain)
		End Function


		Public Overridable Function iterator() As IEnumerator(Of DataSet)
			Return asList().GetEnumerator()
		End Function

		Public Overridable Property FeaturesMaskArray As INDArray Implements org.nd4j.linalg.dataset.api.DataSet.getFeaturesMaskArray
			Get
				Return featuresMask
			End Get
			Set(ByVal featuresMask As INDArray)
				Me.featuresMask = featuresMask
			End Set
		End Property


		Public Overridable Property LabelsMaskArray As INDArray Implements org.nd4j.linalg.dataset.api.DataSet.getLabelsMaskArray
			Get
				Return labelsMask
			End Get
			Set(ByVal labelsMask As INDArray)
				Me.labelsMask = labelsMask
			End Set
		End Property


		Public Overridable Function hasMaskArrays() As Boolean Implements org.nd4j.linalg.dataset.api.DataSet.hasMaskArrays
			Return labelsMask IsNot Nothing OrElse featuresMask IsNot Nothing
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If Not (TypeOf o Is DataSet) Then
				Return False
			End If

			Dim d As DataSet = DirectCast(o, DataSet)

			If Not equalOrBothNull(features_Conflict, d.features_Conflict) Then
				Return False
			End If
			If Not equalOrBothNull(labels_Conflict, d.labels_Conflict) Then
				Return False
			End If
			If Not equalOrBothNull(featuresMask, d.featuresMask) Then
				Return False
			End If
			Return equalOrBothNull(labelsMask, d.labelsMask)
		End Function

		Private Shared Function equalOrBothNull(ByVal first As INDArray, ByVal second As INDArray) As Boolean
			If first Is Nothing AndAlso second Is Nothing Then
				Return True 'Both are null: ok
			End If
			If first Is Nothing OrElse second Is Nothing Then
				Return False 'Only one is null, not both
			End If
			Return first.Equals(second)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = If(Features IsNot Nothing, Features.GetHashCode(), 0)
			result = 31 * result + (If(Labels IsNot Nothing, Labels.GetHashCode(), 0))
			result = 31 * result + (If(FeaturesMaskArray IsNot Nothing, FeaturesMaskArray.GetHashCode(), 0))
			result = 31 * result + (If(LabelsMaskArray IsNot Nothing, LabelsMaskArray.GetHashCode(), 0))
			Return result
		End Function

		''' <summary>
		''' This method returns memory used by this DataSet
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property MemoryFootprint As Long Implements org.nd4j.linalg.dataset.api.DataSet.getMemoryFootprint
			Get
				Dim reqMem As Long = features_Conflict.length() * Nd4j.sizeOfDataType(features_Conflict.dataType())
				reqMem += If(labels_Conflict Is Nothing, 0, labels_Conflict.length() * Nd4j.sizeOfDataType(labels_Conflict.dataType()))
				reqMem += If(featuresMask Is Nothing, 0, featuresMask.length() * Nd4j.sizeOfDataType(featuresMask.dataType()))
				reqMem += If(labelsMask Is Nothing, 0, labelsMask.length() * Nd4j.sizeOfDataType(labelsMask.dataType()))
    
				Return reqMem
			End Get
		End Property

		Public Overridable Sub migrate() Implements org.nd4j.linalg.dataset.api.DataSet.migrate
			If Nd4j.MemoryManager.CurrentWorkspace IsNot Nothing Then
				If features_Conflict IsNot Nothing Then
					features_Conflict = features_Conflict.migrate()
				End If

				If labels_Conflict IsNot Nothing Then
					labels_Conflict = labels_Conflict.migrate()
				End If

				If featuresMask IsNot Nothing Then
					featuresMask = featuresMask.migrate()
				End If

				If labelsMask IsNot Nothing Then
					labelsMask = labelsMask.migrate()
				End If
			End If
		End Sub

		Public Overridable Sub detach() Implements org.nd4j.linalg.dataset.api.DataSet.detach
			If features_Conflict IsNot Nothing Then
				features_Conflict = features_Conflict.detach()
			End If

			If labels_Conflict IsNot Nothing Then
				labels_Conflict = labels_Conflict.detach()
			End If

			If featuresMask IsNot Nothing Then
				featuresMask = featuresMask.detach()
			End If

			If labelsMask IsNot Nothing Then
				labelsMask = labelsMask.detach()
			End If
		End Sub

		Public Overridable ReadOnly Property Empty As Boolean Implements org.nd4j.linalg.dataset.api.DataSet.isEmpty
			Get
				Return features_Conflict Is Nothing AndAlso labels_Conflict Is Nothing AndAlso featuresMask Is Nothing AndAlso labelsMask Is Nothing
			End Get
		End Property

		Public Overridable Function toMultiDataSet() As MultiDataSet
			Dim f As INDArray = Features
			Dim l As INDArray = Labels
			Dim fMask As INDArray = FeaturesMaskArray
			Dim lMask As INDArray = LabelsMaskArray

			Dim fNew() As INDArray = If(f Is Nothing, Nothing, New INDArray()){f}
			Dim lNew() As INDArray = If(l Is Nothing, Nothing, New INDArray()){l}
			Dim fMaskNew() As INDArray = (If(fMask IsNot Nothing, New INDArray() {fMask}, Nothing))
			Dim lMaskNew() As INDArray = (If(lMask IsNot Nothing, New INDArray() {lMask}, Nothing))

			Return New org.nd4j.linalg.dataset.MultiDataSet(fNew, lNew, fMaskNew, lMaskNew, exampleMetaData_Conflict)
		End Function


	End Class

End Namespace