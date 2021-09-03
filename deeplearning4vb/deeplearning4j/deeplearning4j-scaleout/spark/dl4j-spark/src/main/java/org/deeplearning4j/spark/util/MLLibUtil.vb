Imports System
Imports System.Collections.Generic
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports [Function] = org.apache.spark.api.java.function.Function
Imports PortableDataStream = org.apache.spark.input.PortableDataStream
Imports Matrices = org.apache.spark.mllib.linalg.Matrices
Imports Matrix = org.apache.spark.mllib.linalg.Matrix
Imports Vector = org.apache.spark.mllib.linalg.Vector
Imports Vectors = org.apache.spark.mllib.linalg.Vectors
Imports LabeledPoint = org.apache.spark.mllib.regression.LabeledPoint
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports InputStreamInputSplit = org.datavec.api.split.InputStreamInputSplit
Imports Writable = org.datavec.api.writable.Writable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports FeatureUtil = org.nd4j.linalg.util.FeatureUtil
Imports Tuple2 = scala.Tuple2

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

Namespace org.deeplearning4j.spark.util



	''' <summary>
	''' Dl4j <----> MLLib
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class MLLibUtil


		Private Sub New()
		End Sub

		''' <summary>
		''' This is for the edge case where
		''' you have a single output layer
		''' and need to convert the output layer to
		''' an index </summary>
		''' <param name="vector"> the vector to get the classifier prediction for </param>
		''' <returns> the prediction for the given vector </returns>
		Public Shared Function toClassifierPrediction(ByVal vector As Vector) As Double
			Dim max As Double = Double.NegativeInfinity
			Dim maxIndex As Integer = 0
			For i As Integer = 0 To vector.size() - 1
				Dim curr As Double = vector.apply(i)
				If curr > max Then
					maxIndex = i
					max = curr
				End If
			Next i

			Return maxIndex
		End Function

		''' <summary>
		''' Convert an ndarray to a matrix.
		''' Note that the matrix will be con </summary>
		''' <param name="arr"> the array </param>
		''' <returns> an mllib vector </returns>
		Public Shared Function toMatrix(ByVal arr As Matrix) As INDArray

			' we assume that Matrix always has F order
			Return Nd4j.create(arr.toArray(), New Integer() {arr.numRows(), arr.numCols()}, "f"c)
		End Function

		''' <summary>
		''' Convert an ndarray to a vector </summary>
		''' <param name="arr"> the array </param>
		''' <returns> an mllib vector </returns>
		Public Shared Function toVector(ByVal arr As Vector) As INDArray
			Return Nd4j.create(Nd4j.createBuffer(arr.toArray()))
		End Function


		''' <summary>
		''' Convert an ndarray to a matrix.
		''' Note that the matrix will be con </summary>
		''' <param name="arr"> the array </param>
		''' <returns> an mllib vector </returns>
		Public Shared Function toMatrix(ByVal arr As INDArray) As Matrix
			If Not arr.Matrix Then
				Throw New System.ArgumentException("passed in array must be a matrix")
			End If

			' if arr is a view - we have to dup anyway
			If arr.View Then
				Return Matrices.dense(arr.rows(), arr.columns(), arr.dup("f"c).data().asDouble())
			Else ' if not a view - we must ensure data is F ordered
				Return Matrices.dense(arr.rows(), arr.columns(),If(arr.ordering() = "f"c, arr.data().asDouble(), arr.dup("f"c).data().asDouble()))
			End If
		End Function

		''' <summary>
		''' Convert an ndarray to a vector </summary>
		''' <param name="arr"> the array </param>
		''' <returns> an mllib vector </returns>
		Public Shared Function toVector(ByVal arr As INDArray) As Vector
			If Not arr.Vector Then
				Throw New System.ArgumentException("passed in array must be a vector")
			End If
			If arr.length() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim ret(CInt(arr.length()) - 1) As Double
			For i As Integer = 0 To arr.length() - 1
				ret(i) = arr.getDouble(i)
			Next i

			Return Vectors.dense(ret)
		End Function


		''' <summary>
		''' Convert a traditional sc.binaryFiles
		''' in to something usable for machine learning </summary>
		''' <param name="binaryFiles"> the binary files to convert </param>
		''' <param name="reader"> the reader to use </param>
		''' <returns> the labeled points based on the given rdd </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static org.apache.spark.api.java.JavaRDD<org.apache.spark.mllib.regression.LabeledPoint> fromBinary(org.apache.spark.api.java.JavaPairRDD<String, org.apache.spark.input.PortableDataStream> binaryFiles, final org.datavec.api.records.reader.RecordReader reader)
		Public Shared Function fromBinary(ByVal binaryFiles As JavaPairRDD(Of String, PortableDataStream), ByVal reader As RecordReader) As JavaRDD(Of LabeledPoint)
			Dim records As JavaRDD(Of ICollection(Of Writable)) = binaryFiles.map(New FunctionAnonymousInnerClass(reader))

			Dim ret As JavaRDD(Of LabeledPoint) = records.map(New FunctionAnonymousInnerClass2())
			Return ret
		End Function

		Private Class FunctionAnonymousInnerClass
			Inherits [Function](Of Tuple2(Of String, PortableDataStream), ICollection(Of Writable))

			Private reader As RecordReader

			Public Sub New(ByVal reader As RecordReader)
				Me.reader = reader
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Collection<org.datavec.api.writable.Writable> call(scala.Tuple2<String, org.apache.spark.input.PortableDataStream> stringPortableDataStreamTuple2) throws Exception
			Public Overrides Function [call](ByVal stringPortableDataStreamTuple2 As Tuple2(Of String, PortableDataStream)) As ICollection(Of Writable)
				reader.initialize(New org.datavec.api.Split.InputStreamInputSplit(stringPortableDataStreamTuple2._2().open(), stringPortableDataStreamTuple2._1()))
				Return reader.next()
			End Function
		End Class

		Private Class FunctionAnonymousInnerClass2
			Inherits [Function](Of ICollection(Of Writable), LabeledPoint)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.apache.spark.mllib.regression.LabeledPoint call(java.util.Collection<org.datavec.api.writable.Writable> writables) throws Exception
			Public Overrides Function [call](ByVal writables As ICollection(Of Writable)) As LabeledPoint
				Return pointOf(writables)
			End Function
		End Class

		''' <summary>
		''' Convert a traditional sc.binaryFiles
		''' in to something usable for machine learning </summary>
		''' <param name="binaryFiles"> the binary files to convert </param>
		''' <param name="reader"> the reader to use </param>
		''' <returns> the labeled points based on the given rdd </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static org.apache.spark.api.java.JavaRDD<org.apache.spark.mllib.regression.LabeledPoint> fromBinary(org.apache.spark.api.java.JavaRDD<scala.Tuple2<String, org.apache.spark.input.PortableDataStream>> binaryFiles, final org.datavec.api.records.reader.RecordReader reader)
		Public Shared Function fromBinary(ByVal binaryFiles As JavaRDD(Of Tuple2(Of String, PortableDataStream)), ByVal reader As RecordReader) As JavaRDD(Of LabeledPoint)
			Return fromBinary(JavaPairRDD.fromJavaRDD(binaryFiles), reader)
		End Function


		''' <summary>
		''' Returns a labeled point of the writables
		''' where the final item is the point and the rest of the items are
		''' features </summary>
		''' <param name="writables"> the writables </param>
		''' <returns> the labeled point </returns>
		Public Shared Function pointOf(ByVal writables As ICollection(Of Writable)) As LabeledPoint
			Dim ret(writables.Count - 2) As Double
			Dim count As Integer = 0
			Dim target As Double = 0
			For Each w As Writable In writables
				If count < writables.Count - 1 Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = Single.Parse(w.toString());
					ret(count) = Single.Parse(w.ToString())
						count += 1
				Else
					target = Single.Parse(w.ToString())
				End If
			Next w

			If target < 0 Then
				Throw New System.InvalidOperationException("Target must be >= 0")
			End If
			Return New LabeledPoint(target, Vectors.dense(ret))
		End Function

		''' <summary>
		''' Convert an rdd
		''' of labeled point
		''' based on the specified batch size
		''' in to data set </summary>
		''' <param name="data"> the data to convert </param>
		''' <param name="numPossibleLabels"> the number of possible labels </param>
		''' <param name="batchSize"> the batch size </param>
		''' <returns> the new rdd </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static org.apache.spark.api.java.JavaRDD<org.nd4j.linalg.dataset.DataSet> fromLabeledPoint(org.apache.spark.api.java.JavaRDD<org.apache.spark.mllib.regression.LabeledPoint> data, final long numPossibleLabels, long batchSize)
		Public Shared Function fromLabeledPoint(ByVal data As JavaRDD(Of LabeledPoint), ByVal numPossibleLabels As Long, ByVal batchSize As Long) As JavaRDD(Of DataSet)

			Dim mappedData As JavaRDD(Of DataSet) = data.map(New FunctionAnonymousInnerClass3(numPossibleLabels))

			Return mappedData.repartition(CInt(mappedData.count() \ batchSize))
		End Function

		Private Class FunctionAnonymousInnerClass3
			Inherits [Function](Of LabeledPoint, DataSet)

			Private numPossibleLabels As Long

			Public Sub New(ByVal numPossibleLabels As Long)
				Me.numPossibleLabels = numPossibleLabels
			End Sub

			Public Overrides Function [call](ByVal lp As LabeledPoint) As DataSet
				Return fromLabeledPoint(lp, numPossibleLabels)
			End Function
		End Class

		''' <summary>
		''' From labeled point </summary>
		''' <param name="sc"> the org.deeplearning4j.spark context used for creating the rdd </param>
		''' <param name="data"> the data to convert </param>
		''' <param name="numPossibleLabels"> the number of possible labels
		''' @return </param>
		''' @deprecated Use <seealso cref="fromLabeledPoint(JavaRDD, Integer)"/> 
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""fromLabeledPoint(JavaRDD, Integer)""/>") public static org.apache.spark.api.java.JavaRDD<org.nd4j.linalg.dataset.DataSet> fromLabeledPoint(org.apache.spark.api.java.JavaSparkContext sc, org.apache.spark.api.java.JavaRDD<org.apache.spark.mllib.regression.LabeledPoint> data, final long numPossibleLabels)
		<Obsolete("Use <seealso cref=""fromLabeledPoint(JavaRDD, Integer)""/>")>
		Public Shared Function fromLabeledPoint(ByVal sc As JavaSparkContext, ByVal data As JavaRDD(Of LabeledPoint), ByVal numPossibleLabels As Long) As JavaRDD(Of DataSet)
			Return data.map(New FunctionAnonymousInnerClass4(numPossibleLabels))
		End Function

		Private Class FunctionAnonymousInnerClass4
			Inherits [Function](Of LabeledPoint, DataSet)

			Private numPossibleLabels As Long

			Public Sub New(ByVal numPossibleLabels As Long)
				Me.numPossibleLabels = numPossibleLabels
			End Sub

			Public Overrides Function [call](ByVal lp As LabeledPoint) As DataSet
				Return fromLabeledPoint(lp, numPossibleLabels)
			End Function
		End Class

		''' <summary>
		''' Convert rdd labeled points to a rdd dataset with continuous features </summary>
		''' <param name="data"> the java rdd labeled points ready to convert </param>
		''' <returns> a JavaRDD<Dataset> with a continuous label </returns>
		''' @deprecated Use <seealso cref="fromContinuousLabeledPoint(JavaRDD)"/> 
		<Obsolete("Use <seealso cref=""fromContinuousLabeledPoint(JavaRDD)""/>")>
		Public Shared Function fromContinuousLabeledPoint(ByVal sc As JavaSparkContext, ByVal data As JavaRDD(Of LabeledPoint)) As JavaRDD(Of DataSet)

			Return data.map(New FunctionAnonymousInnerClass5())
		End Function

		Private Class FunctionAnonymousInnerClass5
			Inherits [Function](Of LabeledPoint, DataSet)

			Public Overrides Function [call](ByVal lp As LabeledPoint) As DataSet
				Return convertToDataset(lp)
			End Function
		End Class

		Private Shared Function convertToDataset(ByVal lp As LabeledPoint) As DataSet
			Dim features As Vector = lp.features()
			Dim label As Double = lp.label()
			Return New DataSet(Nd4j.create(features.toArray()), Nd4j.create(New Double() {label}))
		End Function

		''' <summary>
		''' Convert an rdd of data set in to labeled point </summary>
		''' <param name="sc"> the spark context to use </param>
		''' <param name="data"> the dataset to convert </param>
		''' <returns> an rdd of labeled point </returns>
		''' @deprecated Use <seealso cref="fromDataSet(JavaRDD)"/>
		'''  
		<Obsolete("Use <seealso cref=""fromDataSet(JavaRDD)""/>")>
		Public Shared Function fromDataSet(ByVal sc As JavaSparkContext, ByVal data As JavaRDD(Of DataSet)) As JavaRDD(Of LabeledPoint)

			Return data.map(New FunctionAnonymousInnerClass6())
		End Function

		Private Class FunctionAnonymousInnerClass6
			Inherits [Function](Of DataSet, LabeledPoint)

			Public Overrides Function [call](ByVal pt As DataSet) As LabeledPoint
				Return toLabeledPoint(pt)
			End Function
		End Class

		''' <summary>
		''' Convert a list of dataset in to a list of labeled points </summary>
		''' <param name="labeledPoints"> the labeled points to convert </param>
		''' <returns> the labeled point list </returns>
		Private Shared Function toLabeledPoint(ByVal labeledPoints As IList(Of DataSet)) As IList(Of LabeledPoint)
			Dim ret As IList(Of LabeledPoint) = New List(Of LabeledPoint)()
			For Each point As DataSet In labeledPoints
				ret.Add(toLabeledPoint(point))
			Next point
			Return ret
		End Function

		''' <summary>
		''' Convert a dataset (feature vector) to a labeled point </summary>
		''' <param name="point"> the point to convert </param>
		''' <returns> the labeled point derived from this dataset </returns>
		Private Shared Function toLabeledPoint(ByVal point As DataSet) As LabeledPoint
			If Not point.Features.Vector Then
				Throw New System.ArgumentException("Feature matrix must be a vector")
			End If

			Dim features As Vector = toVector(point.Features.dup())

			Dim label As Double = Nd4j.BlasWrapper.iamax(point.Labels)
			Return New LabeledPoint(label, features)
		End Function

		''' <summary>
		''' Converts a continuous JavaRDD LabeledPoint to a JavaRDD DataSet. </summary>
		''' <param name="data"> JavaRDD LabeledPoint </param>
		''' <returns> JavaRdd DataSet </returns>
		Public Shared Function fromContinuousLabeledPoint(ByVal data As JavaRDD(Of LabeledPoint)) As JavaRDD(Of DataSet)
			Return fromContinuousLabeledPoint(data, False)
		End Function

		''' <summary>
		''' Converts a continuous JavaRDD LabeledPoint to a JavaRDD DataSet. </summary>
		''' <param name="data"> JavaRdd LabeledPoint </param>
		''' <param name="preCache"> boolean pre-cache rdd before operation
		''' @return </param>
		Public Shared Function fromContinuousLabeledPoint(ByVal data As JavaRDD(Of LabeledPoint), ByVal preCache As Boolean) As JavaRDD(Of DataSet)
			If preCache AndAlso Not data.getStorageLevel().useMemory() Then
				data.cache()
			End If
			Return data.map(New FunctionAnonymousInnerClass7())
		End Function

		Private Class FunctionAnonymousInnerClass7
			Inherits [Function](Of LabeledPoint, DataSet)

			Public Overrides Function [call](ByVal lp As LabeledPoint) As DataSet
				Return convertToDataset(lp)
			End Function
		End Class

		''' <summary>
		''' Converts JavaRDD labeled points to JavaRDD datasets. </summary>
		''' <param name="data"> JavaRDD LabeledPoints </param>
		''' <param name="numPossibleLabels"> number of possible labels
		''' @return </param>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static org.apache.spark.api.java.JavaRDD<org.nd4j.linalg.dataset.DataSet> fromLabeledPoint(org.apache.spark.api.java.JavaRDD<org.apache.spark.mllib.regression.LabeledPoint> data, final long numPossibleLabels)
		Public Shared Function fromLabeledPoint(ByVal data As JavaRDD(Of LabeledPoint), ByVal numPossibleLabels As Long) As JavaRDD(Of DataSet)
			Return fromLabeledPoint(data, numPossibleLabels, False)
		End Function

		''' <summary>
		''' Converts JavaRDD labeled points to JavaRDD DataSets. </summary>
		''' <param name="data"> JavaRDD LabeledPoints </param>
		''' <param name="numPossibleLabels"> number of possible labels </param>
		''' <param name="preCache"> boolean pre-cache rdd before operation
		''' @return </param>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static org.apache.spark.api.java.JavaRDD<org.nd4j.linalg.dataset.DataSet> fromLabeledPoint(org.apache.spark.api.java.JavaRDD<org.apache.spark.mllib.regression.LabeledPoint> data, final long numPossibleLabels, boolean preCache)
		Public Shared Function fromLabeledPoint(ByVal data As JavaRDD(Of LabeledPoint), ByVal numPossibleLabels As Long, ByVal preCache As Boolean) As JavaRDD(Of DataSet)
			If preCache AndAlso Not data.getStorageLevel().useMemory() Then
				data.cache()
			End If
			Return data.map(New FunctionAnonymousInnerClass8(numPossibleLabels))
		End Function

		Private Class FunctionAnonymousInnerClass8
			Inherits [Function](Of LabeledPoint, DataSet)

			Private numPossibleLabels As Long

			Public Sub New(ByVal numPossibleLabels As Long)
				Me.numPossibleLabels = numPossibleLabels
			End Sub

			Public Overrides Function [call](ByVal lp As LabeledPoint) As DataSet
				Return fromLabeledPoint(lp, numPossibleLabels)
			End Function
		End Class

		''' <summary>
		''' Convert an rdd of data set in to labeled point. </summary>
		''' <param name="data"> the dataset to convert </param>
		''' <returns> an rdd of labeled point </returns>
		Public Shared Function fromDataSet(ByVal data As JavaRDD(Of DataSet)) As JavaRDD(Of LabeledPoint)
			Return fromDataSet(data, False)
		End Function

		''' <summary>
		''' Convert an rdd of data set in to labeled point. </summary>
		''' <param name="data"> the dataset to convert </param>
		''' <param name="preCache"> boolean pre-cache rdd before operation </param>
		''' <returns> an rdd of labeled point </returns>
		Public Shared Function fromDataSet(ByVal data As JavaRDD(Of DataSet), ByVal preCache As Boolean) As JavaRDD(Of LabeledPoint)
			If preCache AndAlso Not data.getStorageLevel().useMemory() Then
				data.cache()
			End If
			Return data.map(New FunctionAnonymousInnerClass9())
		End Function

		Private Class FunctionAnonymousInnerClass9
			Inherits [Function](Of DataSet, LabeledPoint)

			Public Overrides Function [call](ByVal dataSet As DataSet) As LabeledPoint
				Return toLabeledPoint(dataSet)
			End Function
		End Class


		''' 
		''' <param name="labeledPoints"> </param>
		''' <param name="numPossibleLabels"> </param>
		''' <returns> List of <seealso cref="DataSet"/> </returns>
		Private Shared Function fromLabeledPoint(ByVal labeledPoints As IList(Of LabeledPoint), ByVal numPossibleLabels As Long) As IList(Of DataSet)
			Dim ret As IList(Of DataSet) = New List(Of DataSet)()
			For Each point As LabeledPoint In labeledPoints
				ret.Add(fromLabeledPoint(point, numPossibleLabels))
			Next point
			Return ret
		End Function

		''' 
		''' <param name="point"> </param>
		''' <param name="numPossibleLabels"> </param>
		''' <returns> <seealso cref="DataSet"/> </returns>
		Private Shared Function fromLabeledPoint(ByVal point As LabeledPoint, ByVal numPossibleLabels As Long) As DataSet
			Dim features As Vector = point.features()
			Dim label As Double = point.label()

			' FIXMEL int cast
			Dim fArr() As Double = features.toArray()
			Return New DataSet(Nd4j.create(fArr, New Long(){1, fArr.Length}), FeatureUtil.toOutcomeVector(CInt(Math.Truncate(label)), CInt(numPossibleLabels)))
		End Function


	End Class

End Namespace