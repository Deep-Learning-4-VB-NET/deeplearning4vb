Imports System
Imports System.Collections.Generic
Imports SparkContext = org.apache.spark.SparkContext
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports SparkUtils = org.deeplearning4j.spark.util.SparkUtils
Imports ValidateMultiDataSetFn = org.deeplearning4j.spark.util.data.validation.ValidateMultiDataSetFn
Imports ValidationResultReduceFn = org.deeplearning4j.spark.util.data.validation.ValidationResultReduceFn
Imports ValidateDataSetFn = org.deeplearning4j.spark.util.data.validation.ValidateDataSetFn

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

Namespace org.deeplearning4j.spark.util.data


	Public Class SparkDataValidation

		Private Sub New()
		End Sub

		''' <summary>
		''' Validate DataSet objects saved to the specified directory on HDFS by attempting to load them and checking their
		''' contents. Assumes DataSets were saved using <seealso cref="org.nd4j.linalg.dataset.DataSet.save(OutputStream)"/>.<br>
		''' Note: this method will also consider all files in subdirectories (i.e., is recursive).
		''' </summary>
		''' <param name="sc">   Spark context </param>
		''' <param name="path"> HDFS path of the directory containing the saved DataSet objects </param>
		''' <returns> Results of the validation </returns>
		Public Shared Function validateDataSets(ByVal sc As JavaSparkContext, ByVal path As String) As ValidationResult
			Return validateDataSets(sc, path, True, False, Nothing, Nothing)
		End Function

		''' <summary>
		''' Validate DataSet objects saved to the specified directory on HDFS by attempting to load them and checking their
		''' contents. Assumes DataSets were saved using <seealso cref="org.nd4j.linalg.dataset.DataSet.save(OutputStream)"/>.<br>
		''' This method (optionally) additionally validates the arrays using the specified shapes for the features and labels.
		''' Note: this method will also consider all files in subdirectories (i.e., is recursive).
		''' </summary>
		''' <param name="sc">            Spark context </param>
		''' <param name="path">          HDFS path of the directory containing the saved DataSet objects </param>
		''' <param name="featuresShape"> May be null. If non-null: feature arrays must match the specified shape, for all values with
		'''                      shape > 0. For example, if featuresShape = {-1,10} then the features must be rank 2,
		'''                      can have any size for the first dimension, but must have size 10 for the second dimension. </param>
		''' <param name="labelsShape">   As per featuresShape, but for the labels instead </param>
		''' <returns> Results of the validation </returns>
		Public Shared Function validateDataSets(ByVal sc As JavaSparkContext, ByVal path As String, ByVal featuresShape() As Integer, ByVal labelsShape() As Integer) As ValidationResult
			Return validateDataSets(sc, path, True, False, featuresShape, labelsShape)
		End Function

		''' <summary>
		''' Validate DataSet objects - <b>and delete any invalid DataSets</b> - that have been previously saved to the
		''' specified directory on HDFS by attempting to load them and checking their contents. Assumes DataSets were saved
		''' using <seealso cref="org.nd4j.linalg.dataset.DataSet.save(OutputStream)"/>.<br>
		''' Note: this method will also consider all files in subdirectories (i.e., is recursive).
		''' </summary>
		''' <param name="sc">   Spark context </param>
		''' <param name="path"> HDFS path of the directory containing the saved DataSet objects </param>
		''' <returns> Results of the validation/deletion </returns>
		Public Shared Function deleteInvalidDataSets(ByVal sc As JavaSparkContext, ByVal path As String) As ValidationResult
			Return validateDataSets(sc, path, True, True, Nothing, Nothing)
		End Function

		''' <summary>
		''' Validate DataSet objects - <b>and delete any invalid DataSets</b> - that have been previously saved to the
		''' specified directory on HDFS by attempting to load them and checking their contents. Assumes DataSets were saved
		''' using <seealso cref="org.nd4j.linalg.dataset.DataSet.save(OutputStream)"/>.<br>
		''' This method (optionally) additionally validates the arrays using the specified shapes for the features and labels.
		''' Note: this method will also consider all files in subdirectories (i.e., is recursive).
		''' </summary>
		''' <param name="sc">            Spark context </param>
		''' <param name="path">          HDFS path of the directory containing the saved DataSet objects </param>
		''' <param name="featuresShape"> May be null. If non-null: feature arrays must match the specified shape, for all values with
		'''                      shape > 0. For example, if featuresShape = {-1,10} then the features must be rank 2,
		'''                      can have any size for the first dimension, but must have size 10 for the second dimension. </param>
		''' <param name="labelsShape">   As per featuresShape, but for the labels instead </param>
		''' <returns> Results of the validation </returns>
		Public Shared Function deleteInvalidDataSets(ByVal sc As JavaSparkContext, ByVal path As String, ByVal featuresShape() As Integer, ByVal labelsShape() As Integer) As ValidationResult
			Return validateDataSets(sc, path, True, True, featuresShape, labelsShape)
		End Function


		Protected Friend Shared Function validateDataSets(ByVal sc As SparkContext, ByVal path As String, ByVal recursive As Boolean, ByVal deleteInvalid As Boolean, ByVal featuresShape() As Integer, ByVal labelsShape() As Integer) As ValidationResult
			Return validateDataSets(New JavaSparkContext(sc), path, recursive, deleteInvalid, featuresShape, labelsShape)
		End Function

		Protected Friend Shared Function validateDataSets(ByVal sc As JavaSparkContext, ByVal path As String, ByVal recursive As Boolean, ByVal deleteInvalid As Boolean, ByVal featuresShape() As Integer, ByVal labelsShape() As Integer) As ValidationResult
			Dim paths As JavaRDD(Of String)
			Try
				paths = SparkUtils.listPaths(sc, path, recursive)
			Catch e As IOException
				Throw New Exception("Error listing paths in directory", e)
			End Try

			Dim results As JavaRDD(Of ValidationResult) = paths.map(New ValidateDataSetFn(deleteInvalid, featuresShape, labelsShape))

			Return results.reduce(New ValidationResultReduceFn())
		End Function


		''' <summary>
		''' Validate MultiDataSet objects saved to the specified directory on HDFS by attempting to load them and checking their
		''' contents. Assumes MultiDataSets were saved using <seealso cref="org.nd4j.linalg.dataset.MultiDataSet.save(OutputStream)"/>.<br>
		''' Note: this method will also consider all files in subdirectories (i.e., is recursive).
		''' </summary>
		''' <param name="sc">   Spark context </param>
		''' <param name="path"> HDFS path of the directory containing the saved DataSet objects </param>
		''' <returns> Results of the validation </returns>
		Public Shared Function validateMultiDataSets(ByVal sc As JavaSparkContext, ByVal path As String) As ValidationResult
			Return validateMultiDataSets(sc, path, True, False, -1, -1, Nothing, Nothing)
		End Function

		''' <summary>
		''' Validate MultiDataSet objects saved to the specified directory on HDFS by attempting to load them and checking their
		''' contents. Assumes MultiDataSets were saved using <seealso cref="org.nd4j.linalg.dataset.MultiDataSet.save(OutputStream)"/>.<br>
		''' This method additionally validates that the expected number of feature/labels arrays are present in all MultiDataSet
		''' objects<br>
		''' Note: this method will also consider all files in subdirectories (i.e., is recursive).
		''' </summary>
		''' <param name="sc">               Spark context </param>
		''' <param name="path">             HDFS path of the directory containing the saved DataSet objects </param>
		''' <param name="numFeatureArrays"> Number of feature arrays that are expected for the MultiDataSet (set -1 to not check) </param>
		''' <param name="numLabelArrays">   Number of labels arrays that are expected for the MultiDataSet (set -1 to not check) </param>
		''' <returns> Results of the validation </returns>
		Public Shared Function validateMultiDataSets(ByVal sc As JavaSparkContext, ByVal path As String, ByVal numFeatureArrays As Integer, ByVal numLabelArrays As Integer) As ValidationResult
			Return validateMultiDataSets(sc, path, True, False, numFeatureArrays, numLabelArrays, Nothing, Nothing)
		End Function


		''' <summary>
		''' Validate MultiDataSet objects saved to the specified directory on HDFS by attempting to load them and checking their
		''' contents. Assumes MultiDataSets were saved using <seealso cref="org.nd4j.linalg.dataset.MultiDataSet.save(OutputStream)"/>.<br>
		''' This method (optionally) additionally validates the arrays using the specified shapes for the features and labels.
		''' Note: this method will also consider all files in subdirectories (i.e., is recursive).
		''' </summary>
		''' <param name="sc">            Spark context </param>
		''' <param name="path">          HDFS path of the directory containing the saved DataSet objects </param>
		''' <param name="featuresShape"> May be null. If non-null: feature arrays must match the specified shapes, for all values with
		'''                      shape > 0. For example, if featuresShape = {{-1,10}} then there must be 1 features array,
		'''                      features array 0 must be rank 2, can have any size for the first dimension, but must have
		'''                      size 10 for the second dimension. </param>
		''' <param name="labelsShape">   As per featuresShape, but for the labels instead </param>
		''' <returns> Results of the validation </returns>
		Public Shared Function validateMultiDataSets(ByVal sc As JavaSparkContext, ByVal path As String, ByVal featuresShape As IList(Of Integer()), ByVal labelsShape As IList(Of Integer())) As ValidationResult
			Return validateMultiDataSets(sc, path, True, False, (If(featuresShape Is Nothing, -1, featuresShape.Count)), (If(labelsShape Is Nothing, -1, labelsShape.Count)), featuresShape, labelsShape)
		End Function

		''' <summary>
		''' Validate MultiDataSet objects - <b>and delete any invalid MultiDataSets</b> - that have been previously saved to the
		''' specified directory on HDFS by attempting to load them and checking their contents. Assumes MultiDataSets were saved
		''' using <seealso cref="org.nd4j.linalg.dataset.MultiDataSet.save(OutputStream)"/>.<br>
		''' Note: this method will also consider all files in subdirectories (i.e., is recursive).
		''' </summary>
		''' <param name="sc">   Spark context </param>
		''' <param name="path"> HDFS path of the directory containing the saved DataSet objects </param>
		''' <returns> Results of the validation/deletion </returns>
		Public Shared Function deleteInvalidMultiDataSets(ByVal sc As JavaSparkContext, ByVal path As String) As ValidationResult
			Return validateMultiDataSets(sc, path, True, True, -1, -1, Nothing, Nothing)
		End Function

		''' <summary>
		''' Validate MultiDataSet objects - <b>and delete any invalid MultiDataSets</b> - that have been previously saved
		''' to the specified directory on HDFS by attempting to load them and checking their contents. Assumes MultiDataSets
		''' were saved using <seealso cref="org.nd4j.linalg.dataset.MultiDataSet.save(OutputStream)"/>.<br>
		''' This method (optionally) additionally validates the arrays using the specified shapes for the features and labels,
		''' Note: this method will also consider all files in subdirectories (i.e., is recursive).
		''' </summary>
		''' <param name="sc">            Spark context </param>
		''' <param name="path">          HDFS path of the directory containing the saved DataSet objects </param>
		''' <param name="featuresShape"> May be null. If non-null: feature arrays must match the specified shapes, for all values with
		'''                      shape > 0. For example, if featuresShape = {{-1,10}} then there must be 1 features array,
		'''                      features array 0 must be rank 2, can have any size for the first dimension, but must have
		'''                      size 10 for the second dimension. </param>
		''' <param name="labelsShape">   As per featuresShape, but for the labels instead </param>
		''' <returns> Results of the validation </returns>
		Public Shared Function deleteInvalidMultiDataSets(ByVal sc As JavaSparkContext, ByVal path As String, ByVal featuresShape As IList(Of Integer()), ByVal labelsShape As IList(Of Integer())) As ValidationResult
			Return validateMultiDataSets(sc, path, True, True, (If(featuresShape Is Nothing, -1, featuresShape.Count)), (If(labelsShape Is Nothing, -1, labelsShape.Count)), featuresShape, labelsShape)
		End Function

		''' <summary>
		''' Validate MultiDataSet objects - <b>and delete any invalid MultiDataSets</b> - that have been previously saved
		''' to the specified directory on HDFS by attempting to load them and checking their contents. Assumes MultiDataSets
		''' were saved using <seealso cref="org.nd4j.linalg.dataset.MultiDataSet.save(OutputStream)"/>.<br>
		''' This method (optionally) additionally validates the arrays using the specified shapes for the features and labels.
		''' Note: this method will also consider all files in subdirectories (i.e., is recursive).
		''' </summary>
		''' <param name="sc">               Spark context </param>
		''' <param name="path">             HDFS path of the directory containing the saved DataSet objects </param>
		''' <param name="numFeatureArrays"> Number of feature arrays that are expected for the MultiDataSet (set -1 to not check) </param>
		''' <param name="numLabelArrays">   Number of labels arrays that are expected for the MultiDataSet (set -1 to not check) </param>
		''' <returns> Results of the validation </returns>
		Public Shared Function deleteInvalidMultiDataSets(ByVal sc As JavaSparkContext, ByVal path As String, ByVal numFeatureArrays As Integer, ByVal numLabelArrays As Integer) As ValidationResult
			Return validateMultiDataSets(sc, path, True, True, numFeatureArrays, numLabelArrays, Nothing, Nothing)
		End Function

		Protected Friend Shared Function validateMultiDataSets(ByVal sc As SparkContext, ByVal path As String, ByVal recursive As Boolean, ByVal deleteInvalid As Boolean, ByVal numFeatureArrays As Integer, ByVal numLabelArrays As Integer, ByVal featuresShape As IList(Of Integer()), ByVal labelsShape As IList(Of Integer())) As ValidationResult
			Return validateMultiDataSets(New JavaSparkContext(sc), path, recursive, deleteInvalid, numFeatureArrays, numLabelArrays, featuresShape, labelsShape)
		End Function

		Protected Friend Shared Function validateMultiDataSets(ByVal sc As JavaSparkContext, ByVal path As String, ByVal recursive As Boolean, ByVal deleteInvalid As Boolean, ByVal numFeatureArrays As Integer, ByVal numLabelArrays As Integer, ByVal featuresShape As IList(Of Integer()), ByVal labelsShape As IList(Of Integer())) As ValidationResult
			Dim paths As JavaRDD(Of String)
			Try
				paths = SparkUtils.listPaths(sc, path, recursive)
			Catch e As IOException
				Throw New Exception("Error listing paths in directory", e)
			End Try

			Dim results As JavaRDD(Of ValidationResult) = paths.map(New ValidateMultiDataSetFn(deleteInvalid, numFeatureArrays, numLabelArrays, featuresShape, labelsShape))

			Return results.reduce(New ValidationResultReduceFn())
		End Function


	End Class

End Namespace