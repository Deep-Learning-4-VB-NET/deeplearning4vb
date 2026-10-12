Imports System
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Eigen = org.nd4j.linalg.eigen.Eigen
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms

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

Namespace org.nd4j.linalg.dimensionalityreduction

	Public Class PCA

'JAVA TO VB CONVERTER NOTE: The field covarianceMatrix was renamed since Visual Basic does not allow fields to have the same name as other class members:
'JAVA TO VB CONVERTER NOTE: The field mean was renamed since Visual Basic does not allow fields to have the same name as other class members:
'JAVA TO VB CONVERTER NOTE: The field eigenvectors was renamed since Visual Basic does not allow fields to have the same name as other class members:
'JAVA TO VB CONVERTER NOTE: The field eigenvalues was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private covarianceMatrix_Conflict, mean_Conflict, eigenvectors_Conflict, eigenvalues_Conflict As INDArray

		Private Sub New()
		End Sub

		''' <summary>
		''' Create a PCA instance with calculated data: covariance, mean, eigenvectors, and eigenvalues. </summary>
		''' <param name="dataset"> The set of data (records) of features, each row is a data record and each
		'''                column is a feature, every data record has the same number of features. </param>
		Public Sub New(ByVal dataset As INDArray)
			Dim covmean() As INDArray = covarianceMatrix(dataset)
			Me.covarianceMatrix_Conflict = covmean(0)
			Me.mean_Conflict = covmean(1)
			Dim pce() As INDArray = principalComponents(covmean(0))
			Me.eigenvectors_Conflict = pce(0)
			Me.eigenvalues_Conflict = pce(1)
		End Sub


		''' <summary>
		''' Return a reduced basis set that covers a certain fraction of the variance of the data </summary>
		''' <param name="variance"> The desired fractional variance (0 to 1), it will always be greater than the value. </param>
		''' <returns> The basis vectors as columns, size <i>N</i> rows by <i>ndims</i> columns, where <i>ndims</i> is less than or equal to <i>N</i> </returns>
		Public Overridable Function reducedBasis(ByVal variance As Double) As INDArray
			Dim vars As INDArray = Transforms.pow(eigenvalues_Conflict, -0.5, True)
			Dim res As Double = vars.sumNumber().doubleValue()
			Dim total As Double = 0.0
			Dim ndims As Integer = 0
			Dim i As Integer = 0
			Do While i < vars.columns()
				ndims += 1
				total += vars.getDouble(i)
				If total / res > variance Then
					Exit Do
				End If
				i += 1
			Loop
			Dim result As INDArray = Nd4j.create(eigenvectors_Conflict.rows(), ndims)
			For i As Integer = 0 To ndims - 1
				result.putColumn(i, eigenvectors_Conflict.getColumn(i))
			Next i
			Return result
		End Function


		''' <summary>
		''' Takes a set of data on each row, with the same number of features as the constructing data
		''' and returns the data in the coordinates of the basis set about the mean. </summary>
		''' <param name="data"> Data of the same features used to construct the PCA object </param>
		''' <returns> The record in terms of the principal component vectors, you can set unused ones to zero. </returns>
		Public Overridable Function convertToComponents(ByVal data As INDArray) As INDArray
			Dim dx As INDArray = data.subRowVector(mean_Conflict)
			Return Nd4j.tensorMmul(eigenvectors_Conflict.transpose(), dx, New Integer()() {
				New Integer() {1},
				New Integer() {1}
			}).transposei()
		End Function


		''' <summary>
		''' Take the data that has been transformed to the principal components about the mean and
		''' transform it back into the original feature set.  Make sure to fill in zeroes in columns
		''' where components were dropped! </summary>
		''' <param name="data"> Data of the same features used to construct the PCA object but as the components </param>
		''' <returns> The records in terms of the original features </returns>
		Public Overridable Function convertBackToFeatures(ByVal data As INDArray) As INDArray
			Return Nd4j.tensorMmul(eigenvectors_Conflict, data, New Integer()() {
				New Integer() {1},
				New Integer() {1}
			}).transposei().addiRowVector(mean_Conflict)
		End Function


		''' <summary>
		''' Estimate the variance of a single record with reduced # of dimensions. </summary>
		''' <param name="data"> A single record with the same <i>N</i> features as the constructing data set </param>
		''' <param name="ndims"> The number of dimensions to include in calculation </param>
		''' <returns> The fraction (0 to 1) of the total variance covered by the <i>ndims</i> basis set. </returns>
		Public Overridable Function estimateVariance(ByVal data As INDArray, ByVal ndims As Integer) As Double
			Dim dx As INDArray = data.sub(mean_Conflict)
			Dim v As INDArray = eigenvectors_Conflict.transpose().mmul(dx.reshape(ChrW(dx.columns()), 1))
			Dim t2 As INDArray = Transforms.pow(v, 2)
			Dim fraction As Double = t2.get(NDArrayIndex.interval(0, ndims)).sumNumber().doubleValue()
			Dim total As Double = t2.sumNumber().doubleValue()
			Return fraction / total
		End Function


		''' <summary>
		''' Generates a set of <i>count</i> random samples with the same variance and mean and eigenvector/values
		''' as the data set used to initialize the PCA object, with same number of features <i>N</i>. </summary>
		''' <param name="count"> The number of samples to generate </param>
		''' <returns> A matrix of size <i>count</i> rows by <i>N</i> columns </returns>
		Public Overridable Function generateGaussianSamples(ByVal count As Long) As INDArray
			Dim samples As INDArray = Nd4j.randn(New Long() {count, eigenvalues_Conflict.columns()})
			Dim factors As INDArray = Transforms.pow(eigenvalues_Conflict, -0.5, True)
			samples.muliRowVector(factors)
			Return Nd4j.tensorMmul(eigenvectors_Conflict, samples, New Integer()() {
				New Integer() {1},
				New Integer() {1}
			}).transposei().addiRowVector(mean_Conflict)
		End Function


		''' <summary>
		''' Calculates pca vectors of a matrix, for a flags number of reduced features
		''' returns the reduced feature set
		''' The return is a projection of A onto principal nDims components
		''' 
		''' To use the PCA: assume A is the original feature set
		''' then project A onto a reduced set of features. It is possible to 
		''' reconstruct the original data ( losing information, but having the same
		''' dimensionality )
		''' 
		''' <pre>
		''' {@code
		''' 
		''' INDArray Areduced = A.mmul( factor ) ;
		''' INDArray Aoriginal = Areduced.mmul( factor.transpose() ) ;
		''' 
		''' }
		''' </pre>
		''' </summary>
		''' <param name="A"> the array of features, rows are results, columns are features - will be changed </param>
		''' <param name="nDims"> the number of components on which to project the features </param>
		''' <param name="normalize"> whether to normalize (adjust each feature to have zero mean) </param>
		''' <returns> the reduced parameters of A </returns>
		Public Shared Function pca(ByVal A As INDArray, ByVal nDims As Integer, ByVal normalize As Boolean) As INDArray
			Dim factor As INDArray = pca_factor(A, nDims, normalize)
			Return A.mmul(factor)
		End Function


		''' <summary>
		''' Calculates pca factors of a matrix, for a flags number of reduced features
		''' returns the factors to scale observations 
		''' 
		''' The return is a factor matrix to reduce (normalized) feature sets
		''' </summary>
		''' <seealso cref= pca(INDArray, int, boolean)
		''' </seealso>
		''' <param name="A"> the array of features, rows are results, columns are features - will be changed </param>
		''' <param name="nDims"> the number of components on which to project the features </param>
		''' <param name="normalize"> whether to normalize (adjust each feature to have zero mean) </param>
		''' <returns> the reduced feature set </returns>
		Public Shared Function pca_factor(ByVal A As INDArray, ByVal nDims As Integer, ByVal normalize As Boolean) As INDArray

			If normalize Then
				' Normalize to mean 0 for each feature ( each column has 0 mean )
				Dim mean As INDArray = A.mean(0)
				A.subiRowVector(mean)
			End If

			Dim m As Long = A.rows()
			Dim n As Long = A.columns()

			' The prepare SVD results, we'll decomp A to UxSxV'
			Dim s As INDArray = Nd4j.create(A.dataType(),If(m < n, m, n))
			Dim VT As INDArray = Nd4j.create(A.dataType(), New Long(){n, n}, "f"c)

			' Note - we don't care about U 
			Nd4j.BlasWrapper.lapack().gesvd(A, s, Nothing, VT)

			' for comparison k & nDims are the equivalent values in both methods implementing PCA

			' So now let's rip out the appropriate number of left singular vectors from
			' the V output (note we pulls rows since VT is a transpose of V)
			Dim V As INDArray = VT.transpose()
			Dim factor As INDArray = Nd4j.create(A.dataType(),New Long(){n, nDims}, "f"c)
			For i As Integer = 0 To nDims - 1
				factor.putColumn(i, V.getColumn(i))
			Next i

			Return factor
		End Function


		''' <summary>
		''' Calculates pca reduced value of a matrix, for a given variance. A larger variance (99%)
		''' will result in a higher order feature set.
		''' 
		''' The returned matrix is a projection of A onto principal components
		''' </summary>
		''' <seealso cref= pca(INDArray, int, boolean)
		''' </seealso>
		''' <param name="A"> the array of features, rows are results, columns are features - will be changed </param>
		''' <param name="variance"> the amount of variance to preserve as a float 0 - 1 </param>
		''' <param name="normalize"> whether to normalize (set features to have zero mean) </param>
		''' <returns> the matrix representing  a reduced feature set </returns>
		Public Shared Function pca(ByVal A As INDArray, ByVal variance As Double, ByVal normalize As Boolean) As INDArray
			Dim factor As INDArray = pca_factor(A, variance, normalize)
			Return A.mmul(factor)
		End Function


		''' <summary>
		''' Calculates pca vectors of a matrix, for a given variance. A larger variance (99%)
		''' will result in a higher order feature set.
		''' 
		''' To use the returned factor: multiply feature(s) by the factor to get a reduced dimension
		''' 
		''' INDArray Areduced = A.mmul( factor ) ;
		''' 
		''' The array Areduced is a projection of A onto principal components
		''' </summary>
		''' <seealso cref= pca(INDArray, double, boolean)
		''' </seealso>
		''' <param name="A"> the array of features, rows are results, columns are features - will be changed </param>
		''' <param name="variance"> the amount of variance to preserve as a float 0 - 1 </param>
		''' <param name="normalize"> whether to normalize (set features to have zero mean) </param>
		''' <returns> the matrix to mulitiply a feature by to get a reduced feature set </returns>
		Public Shared Function pca_factor(ByVal A As INDArray, ByVal variance As Double, ByVal normalize As Boolean) As INDArray
			If normalize Then
				' Normalize to mean 0 for each feature ( each column has 0 mean )
				Dim mean As INDArray = A.mean(0)
				A.subiRowVector(mean)
			End If

			Dim m As Long = A.rows()
			Dim n As Long = A.columns()

			' The prepare SVD results, we'll decomp A to UxSxV'
			Dim s As INDArray = Nd4j.create(A.dataType(),If(m < n, m, n))
			Dim VT As INDArray = Nd4j.create(A.dataType(), New Long(){n, n}, "f"c)

			' Note - we don't care about U 
			Nd4j.BlasWrapper.lapack().gesvd(A, s, Nothing, VT)

			' Now convert the eigs of X into the eigs of the covariance matrix
			Dim i As Integer = 0
			Do While i < s.length()
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				s.putScalar(i, Math.Sqrt(s.getDouble(i)) / (m - 1))
				i += 1
			Loop

			' Now find how many features we need to preserve the required variance
			' Which is the same percentage as a cumulative sum of the eigenvalues' percentages
			Dim totalEigSum As Double = s.sumNumber().doubleValue() * variance
			Dim k As Integer = -1 ' we will reduce to k dimensions
			Dim runningTotal As Double = 0
			For i As Integer = 0 To s.length() - 1
				runningTotal += s.getDouble(i)
				If runningTotal >= totalEigSum Then ' OK I know it's a float, but what else can we do ?
					k = i + 1 ' we will keep this many features to preserve the reqd. variance
					Exit For
				End If
			Next i
			If k = -1 Then ' if we need everything
				Throw New Exception("No reduction possible for reqd. variance - use smaller variance")
			End If
			' So now let's rip out the appropriate number of left singular vectors from
			' the V output (note we pulls rows since VT is a transpose of V)
			Dim V As INDArray = VT.transpose()
			Dim factor As INDArray = Nd4j.createUninitialized(A.dataType(), New Long(){n, k}, "f"c)
			For i As Integer = 0 To k - 1
				factor.putColumn(i, V.getColumn(i))
			Next i

			Return factor
		End Function


		''' <summary>
		''' This method performs a dimensionality reduction, including principal components
		''' that cover a fraction of the total variance of the system.  It does all calculations
		''' about the mean. </summary>
		''' <param name="in"> A matrix of datapoints as rows, where column are features with fixed number N </param>
		''' <param name="variance"> The desired fraction of the total variance required </param>
		''' <returns> The reduced basis set </returns>
		Public Shared Function pca2(ByVal [in] As INDArray, ByVal variance As Double) As INDArray
			' let's calculate the covariance and the mean
			Dim covmean() As INDArray = covarianceMatrix([in])
			' use the covariance matrix (inverse) to find "force constants" and then break into orthonormal
			' unit vector components
			Dim pce() As INDArray = principalComponents(covmean(0))
			' calculate the variance of each component
			Dim vars As INDArray = Transforms.pow(pce(1), -0.5, True)
			Dim res As Double = vars.sumNumber().doubleValue()
			Dim total As Double = 0.0
			Dim ndims As Integer = 0
			Dim i As Integer = 0
			Do While i < vars.columns()
				ndims += 1
				total += vars.getDouble(i)
				If total / res > variance Then
					Exit Do
				End If
				i += 1
			Loop
			Dim result As INDArray = Nd4j.create([in].columns(), ndims)
			For i As Integer = 0 To ndims - 1
				result.putColumn(i, pce(0).getColumn(i))
			Next i
			Return result
		End Function

		''' <summary>
		''' Returns the covariance matrix of a data set of many records, each with N features.
		''' It also returns the average values, which are usually going to be important since in this
		''' version, all modes are centered around the mean.  It's a matrix that has elements that are
		''' expressed as average dx_i * dx_j (used in procedure) or average x_i * x_j - average x_i * average x_j
		''' </summary>
		''' <param name="in"> A matrix of vectors of fixed length N (N features) on each row </param>
		''' <returns> INDArray[2], an N x N covariance matrix is element 0, and the average values is element 1. </returns>
		Public Shared Function covarianceMatrix(ByVal [in] As INDArray) As INDArray()
			Dim dlength As Long = [in].rows()
			Dim vlength As Long = [in].columns()

			Dim product As INDArray = Nd4j.create(vlength, vlength)
			Dim sum As INDArray = [in].sum(0).divi(dlength)

			For i As Integer = 0 To dlength - 1
				Dim dx1 As INDArray = [in].getRow(i).sub(sum)
				product.addi(dx1.reshape(ChrW(vlength), 1).mmul(dx1.reshape(ChrW(1), vlength)))
			Next i
			product.divi(dlength)
			Return New INDArray() {product, sum}
		End Function

		''' <summary>
		''' Calculates the principal component vectors and their eigenvalues (lambda) for the covariance matrix.
		''' The result includes two things: the eigenvectors (modes) as result[0] and the eigenvalues (lambda)
		''' as result[1].
		''' </summary>
		''' <param name="cov"> The covariance matrix (calculated with the covarianceMatrix(in) method) </param>
		''' <returns> Array INDArray[2] "result".  The principal component vectors in decreasing flexibility are
		'''      the columns of element 0 and the eigenvalues are element 1. </returns>
		Public Shared Function principalComponents(ByVal cov As INDArray) As INDArray()
			Preconditions.checkArgument(cov.Matrix AndAlso cov.Square, "Convariance matrix must be a square matrix: has shape %s", cov.shape())
			Dim result(1) As INDArray
			result(0) = Nd4j.eye(cov.rows())
			result(1) = Eigen.symmetricGeneralizedEigenvalues(result(0), cov, True)
			Return result
		End Function


		Public Overridable ReadOnly Property CovarianceMatrix As INDArray
			Get
				Return covarianceMatrix_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property Mean As INDArray
			Get
				Return mean_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property Eigenvectors As INDArray
			Get
				Return eigenvectors_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property Eigenvalues As INDArray
			Get
				Return eigenvalues_Conflict
			End Get
		End Property

	End Class

End Namespace